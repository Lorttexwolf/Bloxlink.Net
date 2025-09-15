using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink
{
    internal readonly struct CachedValue<T>
    {
        public readonly DateTime CachedAt;
        public readonly T Value;

        public CachedValue(DateTime cachedAt, T value)
        {
            CachedAt = cachedAt;
            Value = value;
        }

        public bool IsExpired(TimeSpan cacheDuration, DateTime? currentTime = null)
        {
            currentTime ??= DateTime.Now;

            return (currentTime - CachedAt) <= cacheDuration;
        }

        public static CachedValue<T> Cache(T value)
        {
            return new CachedValue<T>(DateTime.Now, value);
        }
    }
}
