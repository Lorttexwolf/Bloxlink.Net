using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{

    public class BloxlinkRestClient : IDisposable
    {
        public readonly static Uri BaseUrl = new("https://api.blox.link/v1/");

        protected static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        protected readonly HttpClient _httpClient = new()
        {
            BaseAddress = BaseUrl,
            Timeout = new TimeSpan(0, 0, 3)
        };

        protected readonly SemaphoreSlim _stateLock = new(1, 1);

        protected readonly RestWaiter _waiter = new();

        public BloxlinkRestClient() { }
        public BloxlinkRestClient(TimeSpan timeout)
        {
            this._httpClient.Timeout = timeout;
        }

        private async Task<T> GetAsync<T>(Uri uri, BloxlinkRestRequestOptions? options = null) where T : BloxlinkRestResponse, new()
        {
            options ??= new();

            this._stateLock.Wait();

            HttpResponseMessage res;
            
            while (true)
            {
                res = await _httpClient.GetAsync(uri);

                if (res.IsSuccessStatusCode)
                {
                    // Nothing went wrong, lets continue.
                    break;
                }

                if (options.RetryOnRatelimit && res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    this._waiter.WaitAnother(options.RatelimitInterval);
                    this._waiter.Sleep();
                    continue;
                }

                res.EnsureSuccessStatusCode();
            }

            this._stateLock.Release();

            var content = await res.Content.ReadAsStringAsync();

            BloxlinkRestResponse baseResponseData;

            try
            {
                baseResponseData = JsonSerializer.Deserialize<BloxlinkRestResponse>(content)!;
            }
            catch (Exception ex)
            {
                // TODO: Expand on this error message.
                throw new InvalidOperationException("Failed to serialize BloxlinkRestResponse", ex);
            }

            // Ensure that the base request has no errors.
            baseResponseData.EnsureSuccess();

            var responseData = JsonSerializer.Deserialize<T>(content, SerializerOptions)!;

            // Ensure that the request has no errors.
            responseData.EnsureSuccess();

            return responseData;
        }

        #region Endpoint URIs
        private static UriBuilder GetRobloxUserEndpointUri(ulong discordUserId, ulong? guildId = null)
        {
            var builder = new UriBuilder($"https://api.blox.link/v1/user/{discordUserId}");       

            if (guildId != null)
            {
                builder.AddQueryParameter("guild", guildId.ToString());
            }
            return builder;
        }
        #endregion

        /// <summary>
        /// Gets the Roblox account linked to the given <paramref name="discordUser"/>.
        /// </summary>
        /// <param name="guildId">An optional Discord guild to get a users linked Roblox account in.</param>
        public async Task<BloxlinkRestUserResponse> GetUserAsync(ulong discordUser, ulong? guildId = null, BloxlinkRestRequestOptions? options = null)
        {
            var uri = GetRobloxUserEndpointUri(discordUser, guildId).Uri;
            var user = await this.GetAsync<BloxlinkRestUserResponse>(uri, options);
            return user;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            this._httpClient.Dispose();
            this._stateLock.Dispose();
        }
    }
}
