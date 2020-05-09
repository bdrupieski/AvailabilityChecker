using System.Threading.Tasks;
using AvailabilityChecker.AvailabilityCheck;
using CommandLine;

namespace AvailabilityChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AvailabilityCheckerCommandLineParameters>(args)
                .WithParsed(x => Run(x).GetAwaiter().GetResult());
        }

        private static async Task Run(AvailabilityCheckerCommandLineParameters parameters)
        {
            var checker = new ServiceAvailabilityChecker(
                parameters.ServiceName,
                parameters.UrlToHit,
                parameters.WaitMillisecondsBetweenRequests,
                parameters.SlowResponseTimeThresholdMilliseconds,
                parameters.SlackWebUrlAlertUrl,
                parameters.SlackWebHookAlertFrequencyMilliseconds);

            await checker.StartChecking();
        }
    }

    public class AvailabilityCheckerCommandLineParameters
    {
        [Option("servicename", Required = true, HelpText = "Descriptive name of the service to use in alert messages")]
        public string ServiceName { get; set; }

        [Option("url", Required = true, HelpText = "URL to hit constantly with HTTP GETs")]
        public string UrlToHit { get; set; }

        [Option("waitmilliseconds", Required = true, HelpText = "Milliseconds to wait between starting new requests. e.g. 200 ms means roughly 5 requests per second")]
        public int WaitMillisecondsBetweenRequests { get; set; }

        [Option("slowthreshold", Required = true, HelpText = "Threshold in milliseconds for a slow request")]
        public int SlowResponseTimeThresholdMilliseconds { get; set; }

        [Option("slackwebhookurl", Required = true, HelpText = "Slack webhook alert url")]
        public string SlackWebUrlAlertUrl { get; set; }

        [Option("alertfrequency", Required = true, HelpText = "Threshold in milliseconds for the minimum time between slack webhook invocations")]
        public int SlackWebHookAlertFrequencyMilliseconds { get; set; }
    }
}
