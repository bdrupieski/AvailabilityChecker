using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AvailabilityChecker.Extensions;
using AvailabilityChecker.Notifications;
using AvailabilityChecker.Notifications.Reactive;
using AvailabilityChecker.Notifications.Slack;

namespace AvailabilityChecker.AvailabilityCheck
{
    public class Non200ResponseAlerter : BufferingAlerter<HttpStatusCode>
    {
        private readonly string _serviceName;

        public Non200ResponseAlerter(int samplePeriodMilliseconds, IAlertStrategy alertStrategy, string serviceName)
            : base(samplePeriodMilliseconds, alertStrategy)
        {
            _serviceName = serviceName;
        }

        public static Non200ResponseAlerter BuildWithSlackAlerting(string serviceName, string slackAlertWebHookUrl, int slackAlertWebHookFrequencyMilliseconds)
        {
            var alertStrategy = new CompositeAlertStrategy(new NLogAlertStrategy(), new SlackAlertStrategy(slackAlertWebHookUrl));
            return new Non200ResponseAlerter(slackAlertWebHookFrequencyMilliseconds, alertStrategy, serviceName);
        }

        public override string BuildMessage(IList<HttpStatusCode> context, TimeSpan alertPeriod)
        {
            var groupedByStatusCode = context.GroupBy(x => x).Select(x =>
            {
                var count = x.Count();
                var times = count == 1 ? "time" : "times";
                return $"{x.Key} - {count} {times}";
            }).StringJoin("\n");

            return $"{_serviceName} had *{context.Count}* non-200 responses in the past {alertPeriod.TotalSeconds} seconds.\n" +
                   groupedByStatusCode;
        }
    }
}