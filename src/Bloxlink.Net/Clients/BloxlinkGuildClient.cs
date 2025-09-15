using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Bloxlink.Entities;
using Bloxlink.Exceptions;
using Bloxlink.Extensions;
using Bloxlink.URIs;

namespace Bloxlink.Clients
{
    /// <summary>
    /// An HTTP client used to interact with the Bloxlink Server API.
    /// </summary>
    public class BloxlinkGuildClient : BloxlinkBaseClient
    {
        /// <summary>
        /// Lookup table of Discord Guild Id to Bloxlink API Key used for that guild.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, string> _guildAuthKeys = new();

        private readonly ConcurrentDictionary<string, CachedValue<BloxlinkRobloxUser>> _guildToRobloxAccountCache = new();
        private readonly ConcurrentDictionary<string, CachedValue<BloxlinkDiscordUser>> _robloxToGuildAccountCache = new();

        public BloxlinkGuildClient(string authorizationKey, ulong guildId)
        {
            _guildAuthKeys[guildId] = authorizationKey;
        }
        public BloxlinkGuildClient(IDictionary<ulong, string> guildAuthorizationKeys)
        {
            foreach (var kv in guildAuthorizationKeys)
            {
                _guildAuthKeys[kv.Key] = kv.Value;
            }
        }

        /// <summary>
        /// Fetches the Roblox account linked to <paramref name="discordUserId"/> within a specific Discord guild.
        /// </summary>
        public async Task<BloxlinkRobloxUser> GuildMemberToRoblox(ulong discordUserId, ulong guildId, BloxlinkRequestOptions? options = null, CancellationToken ct = default)
        {
            // Populate default values if none were provided.

            options ??= new BloxlinkRequestOptions();

            // If caching is enabled, attempt to serve from cache first.

            var cacheKey = $"G{guildId}U{discordUserId}";
            if (options.UseCache && _guildToRobloxAccountCache.TryGetValue(cacheKey, out var cached) && !cached.IsExpired(CacheDuration))
            {
                // Cache entry is fresh; return it immediately.
                return cached.Value;
            }

            // Resolve the guild's authorization key.

            var authorizationKey = GetGuildAuthorizationKey(guildId);

            // Construct the endpoint URL using provided URIs helper.

            var endpointURL = BloxlinkURIs.DiscordGuildToRoblox(discordUserId, guildId);

            // Issue the GET and deserialize to the model type directly via the base client.

            var response = await GetAsync<BloxlinkRobloxUser>(endpointURL, authorizationKey, options, ct);

            // Throw on failure (provides API error details and quota).

            response.EnsureSuccess();

            // If caching is enabled, record the fresh value.

            if (options.UseCache)
            {
                _guildToRobloxAccountCache[cacheKey] = CachedValue<BloxlinkRobloxUser>.Cache(response.Data!);
            }

            return response.Data!;
        }

        /// <summary>
        /// Performs a reverse lookup: finds a Discord account related to a given Roblox account in the specified guild.
        /// </summary>
        public async Task<BloxlinkDiscordUser> RobloxToGuildMember(ulong robloxUserId, ulong guildId, BloxlinkRequestOptions? options = null, CancellationToken ct = default)
        {
            // Populate default values if none were provided.

            options ??= new BloxlinkRequestOptions();

            var cacheKey = $"R{robloxUserId}G{guildId}";
            if (options.UseCache && _robloxToGuildAccountCache.TryGetValue(cacheKey, out var cached) && !cached.IsExpired(CacheDuration))
            {
                return cached.Value;
            }

            // Resolve the guild's authorization key.

            var authorizationKey = GetGuildAuthorizationKey(guildId);

            var endpointURL = BloxlinkURIs.RobloxToDiscordGuild(robloxUserId, guildId);

            var response = await GetAsync<BloxlinkDiscordUsers>(endpointURL, authorizationKey, options, ct);

            response.EnsureSuccess();

            try
            {
                var discordUser = new BloxlinkDiscordUser(response.Data.AccountIDs.First());

                if (options.UseCache)
                {
                    _robloxToGuildAccountCache[cacheKey] = CachedValue<BloxlinkDiscordUser>.Cache(discordUser);
                }

                return discordUser;
            }
            catch (BloxlinkResponseException<BloxlinkDiscordUser>)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Any unexpected exceptions are wrapped to preserve quota/meta from the response.

                throw new BloxlinkResponseException<BloxlinkDiscordUsers>(
                    BloxlinkResponse<BloxlinkDiscordUsers>.ConstructFailure(ex.Message, response.QuotaRemaining));
            }
        }

        private string GetGuildAuthorizationKey(ulong guildId)
        {
            if (_guildAuthKeys.TryGetValue(guildId, out var key)) return key;

            throw new ArgumentException($"Guild {guildId} is not authorized by this {nameof(BloxlinkGuildClient)}.", nameof(guildId));
        }
    }
}
