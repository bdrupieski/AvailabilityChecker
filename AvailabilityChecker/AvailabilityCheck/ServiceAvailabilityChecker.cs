using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;

namespace AvailabilityChecker.AvailabilityCheck
{
    public class ServiceAvailabilityChecker
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly string _serviceName;
        private readonly string _urlForGet200Response;
        private readonly int _waitMillisecondsBetweenRequests;

        private readonly HttpClient _httpClient;
        private readonly SlowResponseAlerter _slowResponseAlerter;
        private readonly Non200ResponseAlerter _non200ResponseAlerter;
        private readonly TimeSpan _slowResponseTimeThreshold;

        public ServiceAvailabilityChecker(string serviceName, string urlForGet200Response, int waitMillisecondsBetweenRequests, 
            int slowResponseTimeThresholdMilliseconds, string slackWebhookUrl, int slackAlertWebHookFrequencyMilliseconds)
        {
            _serviceName = serviceName;
            _urlForGet200Response = urlForGet200Response;
            _waitMillisecondsBetweenRequests = waitMillisecondsBetweenRequests;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(2),
            };
            _slowResponseTimeThreshold = TimeSpan.FromMilliseconds(slowResponseTimeThresholdMilliseconds);
            _slowResponseAlerter = SlowResponseAlerter.BuildWithSlackAlerting(_serviceName, _slowResponseTimeThreshold, slackWebhookUrl, slackAlertWebHookFrequencyMilliseconds);
            _non200ResponseAlerter = Non200ResponseAlerter.BuildWithSlackAlerting(_serviceName, slackWebhookUrl, slackAlertWebHookFrequencyMilliseconds);
        }

        public async Task StartChecking()
        {
            int timesChecked = 0;
            var tasks = new Queue<Task<ServiceAvailabilityCheckResult>>();

            _logger.Info($"starting to check {_serviceName} availability");

            await WarmUp();

            while (true)
            {
                if (timesChecked % 10_000 == 0)
                    _logger.Info($"still going checking {_serviceName}");

                tasks.Enqueue(Task.Run(CheckServiceAvailability));

                while (tasks.Count > 0 && tasks.Peek().IsCompleted)
                {
                    var next = tasks.Dequeue();
                    if (next.IsFaulted)
                    {
                        _logger.Error(next.Exception, $"call to check {_serviceName} faulted: {next.Exception?.Message} {next.Exception?.InnerException?.Message}");

                        // let up on it a little
                        await Task.Delay(5000);
                        continue;
                    }

                    var result = next.Result;

                    if (result.HttpStatusCode != HttpStatusCode.OK)
                    {
                        _non200ResponseAlerter.MarkEvent(result.HttpStatusCode);
                        LogAvailabilityCheckResult(result);
                    }

                    if (result.RequestDuration > _slowResponseTimeThreshold)
                    {
                        _slowResponseAlerter.MarkEvent(result.RequestDuration);
                        LogAvailabilityCheckResult(result);
                    }
                }

                await Task.Delay(_waitMillisecondsBetweenRequests);

                if (timesChecked > 300_000)
                    break;

                timesChecked++;
            }

            _logger.Info($"shutting down {_serviceName} availability check");
        }

        private void LogAvailabilityCheckResult(ServiceAvailabilityCheckResult result)
        {
            _logger.Info($"{_serviceName} at {result.RequestStartDate} returned {result.HttpStatusCode} in {result.RequestDuration.TotalMilliseconds:F2} ms");
        }

        private async Task WarmUp()
        {
            for (int i = 0; i < 5; i++)
            {
                await CheckServiceAvailability();
                await Task.Delay(750);
            }
        }

        private async Task<ServiceAvailabilityCheckResult> CheckServiceAvailability()
        {
            var sw = Stopwatch.StartNew();
            var requestStart = DateTime.Now;
            var result = await _httpClient.GetAsync(_urlForGet200Response);
            sw.Stop();

            return new ServiceAvailabilityCheckResult
            {
                RequestStartDate = requestStart,
                RequestDuration = sw.Elapsed,
                HttpStatusCode = result.StatusCode,
            };
        }
    }
}