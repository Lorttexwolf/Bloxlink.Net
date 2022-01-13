using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    // Dont worry about this mess, it will be reworked later into a Queue?
    public class RestWaiter
    {
        private DateTime _waitUntil = DateTime.MinValue;

        [DisallowNull]
        // This is quite the mess...
        public DateTime? WaitUntil
        {
            get
            {
                if (this._waitUntil != DateTime.MinValue && DateTime.UtcNow > this._waitUntil)
                {
                    this._waitUntil = DateTime.MinValue;
                    return null;
                }
                return this._waitUntil == DateTime.MinValue ? null : this._waitUntil;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), $"Cannot set to null");

                this._waitUntil = value.Value;
            }
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
            while (true)
            {
                if (this.IsWaiting)
                {
                    Thread.Sleep(this.WaitTime);
                }
                else
                {
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
