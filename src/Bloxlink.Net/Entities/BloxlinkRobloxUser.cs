using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink.Entities
{
    public struct BloxlinkRobloxUser
    {
        [JsonPropertyName("robloxID")]
        public ulong AccountID { get; init; }

        public BloxlinkRobloxUser(ulong robloxId)
        {
            this.AccountID = robloxId;
        }
        public BloxlinkRobloxUser(string robloxId)
        {
            this.AccountID = ulong.Parse(robloxId);
        }
    }
}
