using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    public class BloxlinkRestUserResponse : BloxlinkRestResponse
    {
        /// <summary>
        /// A Discord users globally linked Roblox account. 
        /// </summary>
        [JsonPropertyName("primaryAccount")]
        public ulong GlobalAccount { get; set; }

        /// <summary>
        /// A Discord users linked Roblox account in a Discord guild.
        /// </summary>
        [JsonPropertyName("matchingAccount")]
        public ulong? GuildAccount { get; set; }

        public override void EnsureSuccess()
        {
            base.EnsureSuccess();

            if (this.GlobalAccount == default)
            {
                throw new InvalidOperationException($"Failed to ensure success of {nameof(BloxlinkRestUserResponse)}, property {nameof(GlobalAccount)} cannot be {default(ulong)}.");
            }
        }
    }
}
