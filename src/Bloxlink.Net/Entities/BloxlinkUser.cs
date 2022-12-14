using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink
{

    public struct BloxlinkUser : IInsurable
    {
        /// <summary>
        /// A Discord users globally linked Roblox account. 
        /// </summary>
        [JsonPropertyName("primaryAccount")]
        public ulong GlobalAccount { get; set; }

        /// <summary>
        /// A Discord users linked Roblox account in a Discord guild.
        /// </summary>
        [JsonPropertyName("robloxId")]
        public ulong? GuildAccount { get; set; }

        internal BloxlinkUser(ulong globalAccount, ulong guildAccount = 0)
        {
            this.GlobalAccount = globalAccount;
            this.GuildAccount = guildAccount;
        }

        void IInsurable.EnsureSuccess()
        {
            if (this.GlobalAccount == default)
            {
                throw new InvalidOperationException($"Failed to ensure success of {nameof(BloxlinkUser)}, property {nameof(GlobalAccount)} cannot be {default(ulong)}.");
            }
        }
    }
}
