using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.Text.Json;

namespace Bloxlink
{
    /// <summary>
    /// Represents Bloxlinks base API response containing data and an optional <see cref="ErrorReasoning"/> when an error occurs.
    /// </summary>
    public class BloxlinkResponse : IInsurable
    {
        /// <summary>
        /// Indicates if the operation was a success.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// An optional reason why <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        [JsonPropertyName("error")]
        public string? ErrorReasoning { get; set; }

        /// <summary>
        /// The amount of requests you will be able to make in the current period.
        /// </summary>
        public int QuotaRemaining { get; set; } = -1;

        /// <summary>
        /// Ensures that the response is <see cref="HttpStatusCode.OK"/>, if not an <see cref="Exception"/> will be thrown.
        /// </summary>
        /// <exception cref="UserNotFoundException"></exception>
        public virtual void EnsureSuccess()
        {
            if (this.Success) return;

            throw this.ErrorReasoning switch
            {
                "This user is not linked with Bloxlink." => new UserNotFoundException(),
                _ => new InvalidOperationException($"Failed to ensure success of {nameof(BloxlinkResponse)}.\nError: {this.ErrorReasoning ?? "None"}"),
            };
        }
    }
}
