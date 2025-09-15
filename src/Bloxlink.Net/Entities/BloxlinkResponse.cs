using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using Bloxlink.Exceptions;

namespace Bloxlink
{
    /// <summary>
    /// Represents the base API response containing data and an optional <see cref="ErrorReasoning"/> when an error occurs.
    /// </summary>
    public readonly struct BloxlinkResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was a success.
        /// </summary>
        [JsonPropertyName("success")]
        public readonly bool Success;

        /// <summary>
        /// An optional reason why <see cref="Success"/> is <see langword="false"/>.
        /// </summary>
        [JsonPropertyName("error")]
        public readonly string? ErrorReasoning;

        /// <summary>
        /// The amount of requests you will be able to make in the current period.
        /// </summary>
        public readonly int QuotaRemaining;

        /// <summary>
        /// The successful Bloxlink result.
        /// </summary>
        public readonly T? Data;

        internal BloxlinkResponse(bool success, string? errorReasoning, int quotaRemaining, T? data)
        {
            Success = success;
            ErrorReasoning = errorReasoning;
            QuotaRemaining = quotaRemaining;
            Data = data!;
        }

        internal static BloxlinkResponse<T> ConstructSuccess(int quotaRemaining, T data)
        {
            return new BloxlinkResponse<T>(true, null, quotaRemaining, data);
        }

        internal static BloxlinkResponse<T> ConstructFailure(string? errorReasoning, int quotaRemaining)
        {
            return new BloxlinkResponse<T>(false, errorReasoning, quotaRemaining, default);
        }

        /// <summary>
        /// Ensures that the response is <see cref="HttpStatusCode.OK"/> and <see cref="Success"/>, if not an <see cref="Exception"/> will be thrown.
        /// </summary>
        /// <exception cref="BloxlinkQuotaExceededException{T}"></exception>
        /// <exception cref="BloxlinkUserNotFoundException{T}"></exception>
        /// <exception cref="BloxlinkResponseException{T}"></exception>
        public void EnsureSuccess()
        {
            if (this.Success) return;

            //if (this.QuotaRemaining == 0)
            //{
            //    throw new BloxlinkQuotaExceededException<T>(this);
            //}

            throw this.ErrorReasoning switch
            {
                "You must provide an api-key" => new BloxlinkNotAuthorizedException(),
                "User not found" => new BloxlinkUserNotFoundException<T>(this),
                _ => new BloxlinkResponseException<T>(this)
            };
        }
    }
}
