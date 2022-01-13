using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink
{
    [Serializable]
    public class BloxlinkUserNotFoundException : Exception
    {
        public BloxlinkUserNotFoundException() : base("User is not linked to Bloxlink.") { }
        public BloxlinkUserNotFoundException(string message) : base(message) { }
        public BloxlinkUserNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
