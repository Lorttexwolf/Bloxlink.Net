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
        [JsonPropertyName("primaryAccount")]
        public ulong GlobalAccount { get; set; }

        [JsonPropertyName("matchingAccount")]
        public ulong? GuildAccount { get; set; }
    }
}
