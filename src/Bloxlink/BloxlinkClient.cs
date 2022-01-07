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
            this._restClient.Dispose();
            this._userCache.Clear();
        }

        /// <summary>
        /// Gets a Bloxlink account linked to a certain <paramref name="discordUser"/>.
        /// </summary>
        /// <returns>The Roblox account which is linked to the given <paramref name="discordUser"/>.</returns>
        public async Task<ulong> GetUser(ulong discordUser, bool cache = true)
        {
            if (cache && this._userCache.TryGetValue(discordUser, out var userId))
            {
                return userId;
            }

            userId = (await this._restClient.GetRobloxUser(discordUser)).GlobalAccount;

            this._userCache.Add(discordUser, userId);

            return userId;
        }
        /// <summary>
        /// Gets a Bloxlink account linked to a certain <paramref name="discordUser"/> in the specified <paramref name="guild"/>.
        /// </summary>
        /// <remarks>
        /// Guild member cache is not implemented.
        /// </remarks>
        /// <returns>The Roblox account which is linked to the given <paramref name="discordUser"/>.</returns>
        public async Task<ulong?> GetUser(ulong discordUser, ulong guild)
        {
            return (await this._restClient.GetRobloxUser(discordUser, guild)).GuildAccount;
        }

    }
}
