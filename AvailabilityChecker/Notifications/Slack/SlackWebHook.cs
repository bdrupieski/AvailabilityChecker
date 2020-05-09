using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace AvailabilityChecker.Notifications.Slack
{
    public class SlackWebHook
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _url;

        public SlackWebHook(string urlWithAccessToken)
        {
            _url = urlWithAccessToken;
        }

        public async Task PostMessage(string text)
        {
            var payload = new WebHookPayload
            {
                Text = text
            };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _url);

                var payloadText = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(payloadText);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"failed to send message {text} to slack using {_url}");
            }
        }

        private class WebHookPayload
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}