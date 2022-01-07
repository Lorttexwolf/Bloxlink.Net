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

                throw new HttpRequestException($"GET {uri}", null, res.StatusCode);
            }

            this._stateLock.Release();

            var content = await res.Content.ReadAsStringAsync();

            BloxlinkRestResponse baseRestResponse;

            try
            {
                baseRestResponse = JsonSerializer.Deserialize<BloxlinkRestResponse>(content)!;
            }
            catch (Exception ex)
            {
                // TODO: Expand on this error message.
                throw new InvalidOperationException("Failed to serialize BloxlinkRestResponse", ex);
            }

            // Ensure that this request has no errors.
            baseRestResponse.EnsureSuccess();

            return JsonSerializer.Deserialize<T>(content, SerializerOptions)!;
        }

        private static UriBuilder BuildRobloxUserUri(ulong discordUserId, ulong? guildId = null)
        {
            var builder = new UriBuilder($"https://api.blox.link/v1/user/{discordUserId}");

            if (guildId != null)
            {
                builder.AddQueryParameter("guild", guildId.ToString());
            }
            return builder;
        }

        public async Task<BloxlinkRestUserResponse> GetRobloxUser(ulong discordUserId, ulong? guildId = null, BloxlinkRestRequestOptions? options = null)
        {
            var uri = BuildRobloxUserUri(discordUserId, guildId).Uri;
            var user = await this.GetAsync<BloxlinkRestUserResponse>(uri, options);
            return user;
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
            // this._stateLock.Dispose();
        }
    }
}
