using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    public record BloxlinkRestRequestOptions
    {
        /// <summary>
        /// Wether or not to retry the request if an <see cref="HttpStatusCode.TooManyRequests"/> code was returned.
        /// </summary>
        public bool RetryOnRatelimit { get; set; } = false;

        /// <summary>
        /// The interval at which the request retires until reciving <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        public TimeSpan RatelimitInterval { get; set; } = TimeSpan.FromSeconds(15);
    }
}
