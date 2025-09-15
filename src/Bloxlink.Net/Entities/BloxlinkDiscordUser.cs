using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.Entities
{
    /// <summary>
    /// A result from reverse searching for a Discord account.
    /// </summary>
    public readonly struct BloxlinkDiscordUser
    {
        /// <summary>
        /// The ID of a Discord account associated with the reverse search.
        /// </summary>
        public ulong AccountID { get; }

        public BloxlinkDiscordUser(ulong accountId)
        {
            this.AccountID = accountId;
        }
    }
}
