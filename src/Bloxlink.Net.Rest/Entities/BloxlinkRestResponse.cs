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
    /// <summary>
    /// Represents the status of a <see cref="BloxlinkRestResponse"/>
    /// </summary>
    public enum BloxlinkRestResponseStatus
    {
        Ok,
        /// <summary>
        /// An error occured during an operation, check <see cref="BloxlinkRestResponse.ErrorReasoning"/> for details.
        /// </summary>
        Error
    }

    /// <summary>
    /// Represents Bloxlinks base API response containing its <see cref="Status"/> and an optional <see cref="ErrorReasoning"/> reasoning.
    /// </summary>
    public class BloxlinkRestResponse : IInsurable
    {
        /// <summary>
        /// The requests status indicating how the operation went.
        /// </summary>
        [JsonPropertyName("status"), JsonConverter(typeof(BloxlinkRestResponseStatusConverter))]
        public BloxlinkRestResponseStatus Status { get; set; }

        /// <summary>
        /// An optional reason why <see cref="Status"/> is not <see cref="BloxlinkRestResponseStatus.Ok"/>
        /// </summary>
        [JsonPropertyName("error")]
        public string? ErrorReasoning { get; set; }

        /// <summary>
        /// Ensures that the response is <see cref="HttpStatusCode.OK"/>, if not an <see cref="Exception"/> will be thrown.
        /// </summary>
        /// <exception cref="BloxlinkUserNotFoundException"></exception>
        public virtual void EnsureSuccess()
        {
            if (this.ErrorReasoning == null) return;

            switch (this.ErrorReasoning)
            {
                case "This user is not linked with Bloxlink.":
                    throw new BloxlinkUserNotFoundException();
                default:
                    throw new InvalidOperationException($"Failed to ensure success of {nameof(BloxlinkRestResponse)}.\nError: {this.ErrorReasoning}");
            }
        }
    }
}
