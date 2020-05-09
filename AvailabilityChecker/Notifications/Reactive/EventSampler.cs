using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace AvailabilityChecker.Notifications.Reactive
{
    /// <summary>
    /// When calls are made to <see cref="Sample"/>, <see cref="Sampled"/> will be
    /// invoked only as often as the configured number of milliseconds, passing
    /// along the most recent <see cref="TContext"/> passed to <see cref="Sample"/>.
    /// </summary>
    public abstract class EventSampler<TContext>
    {
        private readonly Subject<TContext> _subject;
        private readonly TimeSpan _sampleTimespan;

        private int _countOfEventsSinceLastSample;

        protected EventSampler(int samplePeriodMilliseconds)
        {
            _sampleTimespan = TimeSpan.FromMilliseconds(samplePeriodMilliseconds);
            _subject = new Subject<TContext>();

            var events = _subject.AsObservable();
            events.Subscribe(CountEvent);

            var sampledEvents = events.Sample(_sampleTimespan);
            sampledEvents.Subscribe(SampleEvent);
        }

        public void Sample(TContext context)
        {
            _subject.OnNext(context);
        }

        public abstract void Sampled(TContext context, int eventCount, TimeSpan alertPeriod);

        private void CountEvent(TContext context)
        {
            Interlocked.Increment(ref _countOfEventsSinceLastSample);
        }

        private void SampleEvent(TContext context)
        {
            int eventCount = Interlocked.Exchange(ref _countOfEventsSinceLastSample, 0);

            Sampled(context, eventCount, _sampleTimespan);
        }
    }
}