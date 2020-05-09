using System;
using System.Collections.Generic;

namespace AvailabilityChecker.Notifications.Reactive
{
    /// <summary>
    /// Invokes the given alert strategy only as often as the configured number of milliseconds.
    /// </summary>
    public abstract class BufferingAlerter<TContext> : EventBufferer<TContext>
    {
        private readonly IAlertStrategy _alertStrategy;

        protected BufferingAlerter(int samplePeriodMilliseconds, IAlertStrategy alertStrategy) : base(samplePeriodMilliseconds)
        {
            _alertStrategy = alertStrategy;
        }

        public override void OnBufferEmit(IList<TContext> context, TimeSpan alertPeriod)
        {
            if (context == null || context.Count == 0)
                return;

            var message = BuildMessage(context, alertPeriod);
            _alertStrategy.Alert(message);
        }

        public abstract string BuildMessage(IList<TContext> context, TimeSpan alertPeriod);
    }
}