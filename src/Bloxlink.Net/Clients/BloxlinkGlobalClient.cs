using Bloxlink.Entities;
using Bloxlink.Exceptions;
using Bloxlink.URIs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxlink.Clients
{
    public class BloxlinkGlobalClient : BloxlinkBaseClient
    {
        /// <summary>
        /// The API key utilized to access the Bloxlink API.
        /// </summary>
        public string AuthorizationKey { get; }

        private readonly ConcurrentDictionary<ulong, CachedValue<BloxlinkRobloxUser>> _discordToRobloxUserCache = new();
        private readonly ConcurrentDictionary<ulong, CachedValue<BloxlinkDiscordUser>> _robloxToDiscordUserCache = new();

        public BloxlinkGlobalClient(string authorizationKey)
        {
            AuthorizationKey = authorizationKey;
        }

        public async Task<BloxlinkRobloxUser> DiscordToRobloxUser(ulong discordUserId, BloxlinkRequestOptions? options = null, CancellationToken ct = default)
        {
            options ??= new BloxlinkRequestOptions();

            if (options.UseCache && _discordToRobloxUserCache.TryGetValue(discordUserId, out var cachedDiscordUser) && !cachedDiscordUser.IsExpired(this.CacheDuration))
            {
                return cachedDiscordUser.Value;
            }

            var endpointURL = BloxlinkURIs.DiscordToRoblox(discordUserId);

            var response = await GetAsync<BloxlinkRobloxUser>(endpointURL, this.AuthorizationKey, options, ct);

            response.EnsureSuccess();

            if (options.UseCache)
            {
                _discordToRobloxUserCache[discordUserId] = CachedValue<BloxlinkRobloxUser>.Cache(response.Data!);
            }
            return response.Data!;
        }

        public async Task<BloxlinkDiscordUser> RobloxToDiscordUser(ulong robloxUserId, BloxlinkRequestOptions? options = null, CancellationToken ct = default)
        {
            options ??= new BloxlinkRequestOptions();

            if (options.UseCache && _robloxToDiscordUserCache.TryGetValue(robloxUserId, out var cachedDiscordUser) && !cachedDiscordUser.IsExpired(this.CacheDuration))
            {
                return cachedDiscordUser.Value;
            }

            var endpointURL = BloxlinkURIs.RobloxToDiscord(robloxUserId);

            var response = await GetAsync<BloxlinkDiscordUsers>(endpointURL, this.AuthorizationKey, options, ct);

            response.EnsureSuccess();

            try
            {
                var discordUser = new BloxlinkDiscordUser(response.Data.AccountIDs.First());

                if (options.UseCache)
                {
                    _robloxToDiscordUserCache[robloxUserId] = CachedValue<BloxlinkDiscordUser>.Cache(discordUser);
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
    }
}
