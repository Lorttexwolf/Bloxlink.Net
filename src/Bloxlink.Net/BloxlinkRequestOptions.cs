using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Bloxlink
{
    public class BloxlinkRequestOptions
    {
        /// <summary>
        /// Wether or not to retry the request if an <see cref="HttpStatusCode.TooManyRequests"/> code was returned.
        /// </summary>
        public bool RetryOnRatelimit { get; set; } = false;

        /// <summary>
        /// The interval at which the request retires when returned a <see cref="HttpStatusCode.TooManyRequests"/> status code.
        /// </summary>
        public TimeSpan RatelimitInterval { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The maximum number of times a request should attempt to send.
        /// </summary>
        /// <remarks>By default, this is set to two retries.</remarks>
        public int RetryLimit { get; set; } = 2;

        /// <summary>
        /// Wether or not to populate the cache. If <see langword="true"/>, this will help lower your API usage.
        /// </summary>
        /// <remarks>Non-<see langword="async"/> methods can be used to access entries of the cache.</remarks>
        public bool PopulateCache { get; set; } = true;
    }
}
