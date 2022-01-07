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

        public TimeSpan WaitInterval { get; set; } = TimeSpan.FromSeconds(8);

        public BloxlinkRestClient() { }
        public BloxlinkRestClient(TimeSpan timeout)
        {
            this._httpClient.Timeout = timeout;
        }

        private async Task<T> GetAsync<T>(Uri uri, BloxlinkRestRequestOptions? options = null) where T : BloxlinkRestResponse, new()
        {
            options ??= new();

            this._stateLock.Wait();

            HttpResponseMessage res = await _httpClient.GetAsync(uri);

            if (!res.IsSuccessStatusCode)
            {
                if (options.RetryAtTimeout && res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    while (true)
                    {
                        if (this._waiter.IsWaiting)
                        {
                            Trace.WriteLine($"Rest - Request {uri} waiting {this._waiter.WaitTime}");
                            this._waiter.Sleep();
                        }

                        res = await _httpClient.GetAsync(uri);

                        if (res.IsSuccessStatusCode)
                        {
                            // Nothing went wrong, lets get outa here!
                            break;
                        }
                        else if (res.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            this._waiter.WaitAnother(options.TimeoutInterval);
                            Trace.WriteLine($"Rest - Response {uri} Too Many Requests, waiting {this._waiter.WaitTime}");
                        }
                    }
                }
                else if (!options.RetryAtTimeout && res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new HttpRequestException($"Failed to GET {uri}", null, res.StatusCode);
                }
            }

            this._stateLock.Release();

            var content = await res.Content.ReadAsStringAsync();

            BloxlinkRestResponse baseData;

            try
            {
                baseData = JsonSerializer.Deserialize<BloxlinkRestResponse>(content);
            }
            catch (Exception ex)
            {
                // TODO: Expand on message
                throw new InvalidOperationException("Failed to serialize BloxlinkRestResponse", ex);
            }

            baseData.EnsureSuccess();

            var data = JsonSerializer.Deserialize<T>(content, SerializerOptions);

            // Trace.WriteLine($"Completed on Task {Task.CurrentId} on Thread {Thread.CurrentThread.ManagedThreadId}");

            return data;
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
            Trace.WriteLine($"Got User {user}");
            return user;
        }

        public async Task<IEnumerable<BloxlinkRestUserResponse>> GetRobloxUsers(ulong? guildId = null, params ulong[] discordUserIds)
        {
            var tasks = new List<Task<BloxlinkRestUserResponse>>();

            foreach (var discordUserId in discordUserIds)
            {
                tasks.Add(this.GetRobloxUser(discordUserId, guildId));
            }
            return await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
            // this._stateLock.Dispose();
        }
    }
}
