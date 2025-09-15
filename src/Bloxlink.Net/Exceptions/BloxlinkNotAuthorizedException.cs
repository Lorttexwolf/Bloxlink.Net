using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.Exceptions
{
    public class BloxlinkNotAuthorizedException : Exception
    {
        public BloxlinkNotAuthorizedException() : base("Client not authorized to access Bloxlink resource") { }
    }
}
