using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxlink.Clients
{
    public abstract class BloxlinkBaseClient
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true
        };

        private static readonly HttpClient Http = new(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5), // Configured to support high-demand traffic.
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public BloxlinkRequestOptions DefaultRequestOptions { get; set; } = new();

        /// <summary> 
        /// The amount of time an account or relation should be cached. 
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);

        protected async Task<BloxlinkResponse<T>> GetAsync<T>(string url, string authorizationKey, BloxlinkRequestOptions? options = null, CancellationToken ct = default)
        {
            options ??= DefaultRequestOptions;

            var res = await GetAsync(url, authorizationKey, options, ct);
            if (!res.Success) return BloxlinkResponse<T>.ConstructFailure(res.ErrorReasoning, res.QuotaRemaining);

            try
            {
                // Deserialize once from the document’s RootElement

                var model = res.Data!.RootElement.Deserialize<T>(SerializerOptions)!;
                return BloxlinkResponse<T>.ConstructSuccess(res.QuotaRemaining, model);
            }
            catch (JsonException jx)
            {
                return BloxlinkResponse<T>.ConstructFailure($"Failed to deserialize Bloxlink response: {jx.Message}", res.QuotaRemaining);
            }
        }

        protected async Task<BloxlinkResponse<JsonDocument>> GetAsync(string url, string authorizationKey, BloxlinkRequestOptions options, CancellationToken ct = default)
        {
            int attempts = 0;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.TryAddWithoutValidation("Authorization", authorizationKey);
                req.Headers.TryAddWithoutValidation("Accept", "application/json");

                HttpResponseMessage res;
                try
                {
                    res = await Http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                }
                catch (TaskCanceledException) when (!ct.IsCancellationRequested)
                {
                    if (attempts++ < options.RetryLimit)
                    {
                        await Task.Delay(options.RateLimitInterval, ct);
                        continue;
                    }

                    return BloxlinkResponse<JsonDocument>.ConstructFailure("HTTP request timed out.", 0);
                }

                int quotaRemaining = TryGetQuotaRemaining(res);

                // Handle 429 (TooManyRequests)
                if (res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    if (!options.RetryOnRateLimit || attempts++ >= options.RetryLimit)
                    {
                        // v4 may not return quota header when exhausted.
                        return new BloxlinkQuotaExceededException<JsonDocument>(
                            BloxlinkResponse<JsonDocument>.ConstructFailure("Quota exhausted", quotaRemaining)
                        ).Response;
                    }

                    var delay = GetRetryDelay(res, options.RateLimitInterval);

                    await Task.Delay(delay, ct);

                    continue;
                }

                // Retry on transient 5xx

                if ((int)res.StatusCode >= 500 && (int)res.StatusCode <= 599 && attempts++ < options.RetryLimit)
                {
                    await Task.Delay(options.RateLimitInterval, ct);
                    continue;
                }

                // Read content once

                try
                {
                    await using var stream = await res.Content.ReadAsStreamAsync(ct);
                    var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

                    // If API encodes errors in body

                    if (doc.RootElement.TryGetProperty("error", out var errProp) && errProp.ValueKind == JsonValueKind.String)
                    {
                        return BloxlinkResponse<JsonDocument>.ConstructFailure(errProp.GetString(), quotaRemaining);
                    }

                    if (!res.IsSuccessStatusCode)
                    {
                        // Compose a helpful error message

                        var status = (int)res.StatusCode;

                        string detail = doc.RootElement.TryGetProperty("message", out var msgProp) && msgProp.ValueKind == JsonValueKind.String
                                      ? msgProp.GetString()!
                                      : res.ReasonPhrase ?? "Unknown error";

                        return BloxlinkResponse<JsonDocument>.ConstructFailure($"HTTP {status}: {detail}", quotaRemaining);
                    }

                    return BloxlinkResponse<JsonDocument>.ConstructSuccess(quotaRemaining, doc);
                }
                catch (JsonException jx)
                {
                    return BloxlinkResponse<JsonDocument>.ConstructFailure($"Invalid JSON from Bloxlink: {jx.Message}", quotaRemaining);
                }
                catch (Exception ex)
                {
                    return BloxlinkResponse<JsonDocument>.ConstructFailure($"Failed to read response: {ex.Message}", quotaRemaining);
                }
            }
        }

        private static int TryGetQuotaRemaining(HttpResponseMessage res)
        {
            // Bloxlink V4 API does not return quota in headers.

            if (res.Headers.TryGetValues("quota-remaining", out var values))
            {
                var first = values.FirstOrDefault();

                if (int.TryParse(first, out var parsed)) return parsed;
            }
            return 0;
        }

        private static TimeSpan GetRetryDelay(HttpResponseMessage res, TimeSpan fallback)
        {
            if (res.Headers.RetryAfter is { } ra)
            {
                if (ra.Delta is TimeSpan delta) return delta > TimeSpan.Zero ? delta : fallback;
                if (ra.Date is DateTimeOffset dto)
                {
                    var wait = dto - DateTimeOffset.UtcNow;
                    if (wait > TimeSpan.Zero) return wait;
                }
            }
            return fallback;
        }
    }

    public class BloxlinkRequestOptions
    {
        /// <summary>
        /// Whether or not to retry the request if an <see cref="HttpStatusCode.TooManyRequests"/> code was returned.
        /// </summary>
        public bool RetryOnRateLimit { get; set; } = false;

        /// <summary>
        /// The interval at which the request retires when returned a <see cref="HttpStatusCode.TooManyRequests"/> status code.
        /// </summary>
        public TimeSpan RateLimitInterval { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The maximum number of times a request should attempt to send.
        /// </summary>
        /// <remarks>By default, this is set to two retries.</remarks>
        public int RetryLimit { get; set; } = 2;

        /// <summary>
        /// Whether or not to populate the cache. If <see langword="true"/>, this will help lower your API usage.
        /// </summary>
        /// <remarks>Non-<see langword="async"/> methods can be used to access entries of the cache.</remarks>
        public bool UseCache { get; set; } = true;
    }
}
