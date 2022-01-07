using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    public class RestWaiter
    {
        private DateTime _waitUntil = DateTime.MinValue;

        public DateTime? WaitUntil
        {
            get
            {
                if (this._waitUntil != DateTime.MinValue && DateTime.UtcNow > this._waitUntil)
                {
                    Trace.WriteLine($"{nameof(RestWaiter)}\t - Cleared Wait Until");
                    this._waitUntil = DateTime.MinValue;
                    return null;
                }
                if (this._waitUntil == DateTime.MinValue)
                {
                    return null;
                }
                return this._waitUntil;
            }
            set => this._waitUntil = value.Value;
        }

        public TimeSpan WaitTime
        {
            get
            {
                var time = this.WaitUntil;

                if (time != null && DateTime.UtcNow < time)
                {
                    return ((DateTime)time).Subtract(DateTime.UtcNow);
                }
                return TimeSpan.Zero;
            }
        }

        public bool IsWaiting => this.WaitTime != TimeSpan.Zero;


        /// <summary>
        /// Puts the current <see cref="Thread"/> to sleep for <see cref="WaitTime"/>.
        /// </summary>
        public void Sleep()
        {
            if (this.IsWaiting)
            {
                var time = this.WaitTime;

                Trace.WriteLine($"{nameof(RestWaiter)}\t - Thread went to sleep for {time}");
                Thread.Sleep(time);
            }

            while (true)
            {
                Trace.Write($"{nameof(RestWaiter)}\t - Thread woke up...");
                if (this.IsWaiting)
                {
                    var time = this.WaitTime;

                    Trace.WriteLine($" then went back to sleep for {time}");
                    Thread.Sleep(time);
                }
                else
                {
                    Trace.Write(" and stayed up.");
                    break;
                }
            }
        }

        /// <summary>
        /// Waits an amount of <paramref name="time"/>
        /// </summary>
        /// <param name="timeout">The amount of time to wait.</param>
        public void WaitAnother(TimeSpan time)
        {
            var currentTime = this.WaitUntil ?? DateTime.UtcNow;
            var newTime = currentTime.Add(time);

            this._waitUntil = newTime;
        }
    }
}
