using System;

namespace AvailabilityChecker.Notifications.Reactive
{
    /// <summary>
    /// Invokes the given alert strategy only as often as the configured number of milliseconds
    /// for calls made to <see cref="EventSampler{TContext}.Sample"/>.
    /// </summary>
    public abstract class SamplingAlerter<TContext> : EventSampler<TContext>
    {
        private readonly IAlertStrategy _alertStrategy;

        protected SamplingAlerter(int samplePeriodMilliseconds, IAlertStrategy alertStrategy) : base(samplePeriodMilliseconds)
        {
            _alertStrategy = alertStrategy;
        }

        public override void Sampled(TContext context, int eventCount, TimeSpan alertPeriod)
        {
            var message = BuildMessage(context, eventCount, alertPeriod);
            _alertStrategy.Alert(message);
        }

        public abstract string BuildMessage(TContext context, int eventCount, TimeSpan alertPeriod);
    }
}