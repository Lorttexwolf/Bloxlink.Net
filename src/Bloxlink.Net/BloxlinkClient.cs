using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Bloxlink.Extensions;

namespace Bloxlink
{
    public class BloxlinkClient : IDisposable
    {
        protected static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        protected readonly SemaphoreSlim _stateLock = new(1, 1);
        protected readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(2)
        };

        private Dictionary<BloxlinkUserLookup, ulong> _userCache { get; init; } = new();

        public BloxlinkClient(string apiKey)
        {
            this._httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
        }

        #region Endpoint URIs
        private static UriBuilder GetRobloxUserEndpointUri(ulong discordUserId, ulong? guildId = null)
        {
            var builder = new UriBuilder($"https://v3.blox.link/developer/discord/{discordUserId}");

            if (guildId != null) builder.AddQueryParameter("guildId", guildId.ToString());
            return builder;
        }
        #endregion

        /// <summary>
        /// Retrives a cached user from the cache. If not cached, <see langword="null"/> will be returned.
        /// </summary>
        public ulong? GetUser(ulong discordUser, ulong? guildId = 0)
        {
            if (this._userCache.TryGetValue(new BloxlinkUserLookup(discordUser, guildId), out var cachedUser)) return cachedUser;
            return null;
        }

        /// <summary>
        /// Gets the Roblox account linked to the given <paramref name="discordUser"/>.
        /// </summary>
        /// <param name="guildId">An optional Discord guild to get a users linked Roblox account in.</param>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="QuotaExceededException"></exception>
        public async Task<BloxlinkUserResponse> GetUserAsync(ulong discordUser, ulong? guildId = null, BloxlinkRequestOptions? options = null)
        {
            options ??= new();

            var uri = GetRobloxUserEndpointUri(discordUser, guildId).Uri;
            Console.WriteLine(uri);
            BloxlinkUserResponse res;
            try
            {
                res = await this.GetAsync<BloxlinkUserResponse>(uri, options);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound) throw new UserNotFoundException();
                throw ex;
            }

            if (options.PopulateCache)
            {
                // Add the result to the cache.
                _userCache.TryAdd(new BloxlinkUserLookup(discordUser, guildId), (ulong)res.User.GuildAccount!);
            }
            return res;
        }

        /// <summary>
        /// Throws an <see cref="Exception"/> if the provided api-key is invalid.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public async Task ValidateKey()
        {
            // Cheeky way to test the key and doesn't decrement quota! 
            var res = await this._httpClient.GetAsync("https://v3.blox.link/developer/discord/0", HttpCompletionOption.ResponseHeadersRead);
            if (res.StatusCode != HttpStatusCode.NotFound)
                throw new Exception("Provided api-key is invalid.");
        }

        protected async Task<T> GetAsync<T>(Uri uri, BloxlinkRequestOptions? options = null) where T : BloxlinkResponse, new()
        {
            options ??= new();

            this._stateLock.Wait();

            HttpResponseMessage res;
            int attempts = 0;

            while (true)
            {
                if (attempts > options.RetryLimit) throw new HttpRequestException($"Request exceeded {options.RetryLimit} attempts.");

                res = await _httpClient.GetAsync(uri);
                if (res.IsSuccessStatusCode) break;

                if (res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    // The `quota-remaining` header will not be provided once exceeding.
                    if (!res.Headers.Has("quota-remaining")) throw new QuotaExceededException();

                    if (options.RetryOnRatelimit)
                    {
                        Thread.Sleep(options.RatelimitInterval);
                        attempts++;
                        continue;
                    }
                }
                else res.EnsureSuccessStatusCode();
            }
            this._stateLock.Release();

            T responseData;
            try
            {
                responseData = (await res.Content.ReadFromJsonAsync<T>(SerializerOptions))!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to serialize BloxlinkResponse", ex);
            }
            responseData.QuotaRemaining = Convert.ToInt32(res.Headers.GetValues("quota-remaining").First());

            // Ensure that the request has no errors.
            responseData.EnsureSuccess();
            return responseData;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            this._httpClient.Dispose();
            this._stateLock.Dispose();
            this._userCache.Clear();
        }
    }
}
