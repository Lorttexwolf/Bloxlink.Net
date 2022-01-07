using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    [Serializable]
    public class BloxlinkRestUserNotFoundException : Exception
    {
        public ulong? RobloxUserId { get; init; }

        public BloxlinkRestUserNotFoundException() : base("User is not linked to Bloxlink.Rest") { }
        public BloxlinkRestUserNotFoundException(string message) : base(message) { }
        public BloxlinkRestUserNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
