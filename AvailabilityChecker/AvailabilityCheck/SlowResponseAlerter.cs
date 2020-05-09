using System;
using System.Collections.Generic;
using System.Linq;
using AvailabilityChecker.Extensions;
using AvailabilityChecker.Notifications;
using AvailabilityChecker.Notifications.Reactive;
using AvailabilityChecker.Notifications.Slack;

namespace AvailabilityChecker.AvailabilityCheck
{
    public class SlowResponseAlerter : BufferingAlerter<TimeSpan>
    {
        private readonly string _serviceName;
        private readonly TimeSpan _slowResponseThreshold;

        public SlowResponseAlerter(int samplePeriodMilliseconds, IAlertStrategy alertStrategy, string serviceName, TimeSpan slowResponseThreshold)
            : base(samplePeriodMilliseconds, alertStrategy)
        {
            _serviceName = serviceName;
            _slowResponseThreshold = slowResponseThreshold;
        }

        public static SlowResponseAlerter BuildWithSlackAlerting(string serviceName, TimeSpan slowResponseThreshold, string slackAlertWebHookUrl, int slackAlertWebHookFrequencyMilliseconds)
        {
            var alertStrategy = new CompositeAlertStrategy(new NLogAlertStrategy(), new SlackAlertStrategy(slackAlertWebHookUrl));
            return new SlowResponseAlerter(slackAlertWebHookFrequencyMilliseconds, alertStrategy, serviceName, slowResponseThreshold);
        }

        public override string BuildMessage(IList<TimeSpan> context, TimeSpan alertPeriod)
        {
            var times = context.Count == 1 ? "time" : "times";
            var maxTimeSpan = context.Max(x => x);
            var minTimeSpan = context.Min(x => x);
            var avg = context.Average(x => x.TotalMilliseconds);
            var median = context.Median(x => x.TotalMilliseconds);

            return $"{_serviceName} took >{_slowResponseThreshold.TotalSeconds}s to respond *{context.Count}* {times} in the past {alertPeriod.TotalSeconds}s.\n" +
                   $"Min: {minTimeSpan.TotalMilliseconds:N0} ms Max: {maxTimeSpan.TotalMilliseconds:N0} ms Avg: {avg:N0} ms Median: {median:N0} ms";
        }
    }
}