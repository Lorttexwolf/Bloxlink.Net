using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bloxlink.Rest;

namespace Bloxlink
{
    public class BloxlinkClient : IDisposable
    {
        protected readonly BloxlinkRestClient _restClient = new();

        protected readonly IDictionary<ulong, ulong> _userCache = new Dictionary<ulong, ulong>();

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            this._restClient.Dispose();
            this._userCache.Clear();
        }

        /// <summary>
        /// Gets a Bloxlink account linked to a certain <paramref name="discordUser"/>.
        /// </summary>
        /// <returns>The Roblox account which is linked to the given <paramref name="discordUser"/>.</returns>
        public async Task<ulong> GetUserAsync(ulong discordUser, bool cache = true, BloxlinkRestRequestOptions? options = null)
        {
            if (cache && this._userCache.TryGetValue(discordUser, out var userId))
            {
                return userId;
            }

            userId = (await this._restClient.GetUserAsync(discordUser, options: options)).GlobalAccount;

            if (cache) this._userCache.TryAdd(discordUser, userId);

            return userId;
        }
        /// <summary>
        /// Gets a Bloxlink account linked to a certain <paramref name="discordUser"/> in the specified <paramref name="guild"/>.
        /// </summary>
        /// <remarks>
        /// Guild member cache is not implemented.
        /// </remarks>
        /// <returns>The Roblox account which is linked to the given <paramref name="discordUser"/>.</returns>
        public async Task<ulong?> GetUserAsync(ulong discordUser, ulong guild, BloxlinkRestRequestOptions? options = null)
        {
            return (await this._restClient.GetUserAsync(discordUser, guild, options)).GuildAccount;
        }

    }
}
