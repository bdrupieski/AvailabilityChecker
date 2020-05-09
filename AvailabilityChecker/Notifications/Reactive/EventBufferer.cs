using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AvailabilityChecker.Notifications.Reactive
{
    /// <summary>
    /// When calls are made to <see cref="MarkEvent"/>, <see cref="OnBufferEmit"/>
    /// will be invoked only as often as the configured number of milliseconds,
    /// passing along a list of all the marked events over that previous number
    /// of milliseconds.
    /// </summary>
    public abstract class EventBufferer<TContext>
    {
        private readonly Subject<TContext> _subject;
        private readonly TimeSpan _bufferTimeSpan;

        protected EventBufferer(int bufferPeriodMilliseconds)
        {
            _bufferTimeSpan = TimeSpan.FromMilliseconds(bufferPeriodMilliseconds);
            _subject = new Subject<TContext>();

            var events = _subject.AsObservable();
            var bufferedEvents = events.Buffer(_bufferTimeSpan);
            bufferedEvents.Subscribe(EmitBuffer);
        }

        public void MarkEvent(TContext context)
        {
            _subject.OnNext(context);
        }

        public abstract void OnBufferEmit(IList<TContext> context, TimeSpan alertPeriod);

        private void EmitBuffer(IList<TContext> context)
        {
            OnBufferEmit(context, _bufferTimeSpan);
        }
    }
}