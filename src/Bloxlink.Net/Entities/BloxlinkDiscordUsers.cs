using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink.Entities
{
    /// <summary>
    /// A result from reverse searching for multiple Discord accounts.
    /// </summary>
    internal readonly struct BloxlinkDiscordUsers
    {
        /// <summary>
        /// The ID(s) of the Discord account(s) associated with the reverse search.
        /// </summary>
        [JsonPropertyName("discordIDs")]
        public ulong[] AccountIDs { get; init; }

        public BloxlinkDiscordUsers(ulong[] accountIds)
        {
            this.AccountIDs = accountIds;
        }
    }
}
