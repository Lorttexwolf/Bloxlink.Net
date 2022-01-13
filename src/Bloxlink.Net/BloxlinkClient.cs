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
        private readonly BloxlinkRestClient restClient = new();
            
        public IDictionary<ulong, ulong> LinkedRobloxUserCache { get; init; } = new Dictionary<ulong, ulong>();

        /// <summary>
        /// Gets the Roblox account linked to the given <paramref name="discordUser"/>.
        /// </summary>
        public async Task<ulong> GetUserAsync(ulong discordUser, bool cache = true, BloxlinkRestRequestOptions? options = null)
        {
            if (cache && this.LinkedRobloxUserCache.TryGetValue(discordUser, out var userId))
            {
                return userId;
            }

            userId = (await this.restClient.GetUserAsync(discordUser, options: options)).GlobalAccount;

            if (cache) this.LinkedRobloxUserCache.TryAdd(discordUser, userId);

            return userId;
        }


        /// <summary>
        /// Gets a Roblox account linked to the <paramref name="discordUser"/> in the specified <paramref name="guild"/>.
        /// </summary>
        /// <remarks>
        /// Guild member cache is not implemented.
        /// </remarks>
        public async Task<ulong?> GetUserAsync(ulong discordUser, ulong guild, BloxlinkRestRequestOptions? options = null)
        {
            return (await this.restClient.GetUserAsync(discordUser, guild, options)).GuildAccount;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            this.restClient.Dispose();
            this.LinkedRobloxUserCache.Clear();
        }
    }
}
