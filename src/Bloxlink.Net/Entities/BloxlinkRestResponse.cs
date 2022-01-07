using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.Text.Json;

namespace Bloxlink.Rest
{
    public class BloxlinkRestRequestOptions
    {
        /// <summary>
        /// Wether or not to retry the request if an <see cref="HttpStatusCode.TooManyRequests"/> code was returned.
        /// </summary>
        public bool RetryAtTimeout { get; set; } = false;

        /// <summary>
        /// The interval at which timeout retires will be sent until <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        public TimeSpan TimeoutInterval { get; set; } = TimeSpan.FromSeconds(15);
    }

    public class BloxlinkRestResponse
    {
        public enum StatusType
        {
            Ok,
            Error
        }

        [JsonPropertyName("status"), JsonConverter(typeof(BloxlinkRestResponseStatusConverter))]
        public StatusType Status { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Ensures that the response is <see cref="HttpStatusCode.OK"/>, if not an <see cref="Exception"/> will be thrown.
        /// </summary>
        /// <exception cref="BloxlinkRestUserNotFoundException"></exception>
        public void EnsureSuccess()
        {
            switch (this.Error)
            {
                case "This user is not linked with Bloxlink.Rest.":
                    throw new BloxlinkRestUserNotFoundException();
            }
        }
    }
}
