using Bloxlink.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink
{
    [Serializable]
    public class BloxlinkUserNotFoundException<T> : BloxlinkResponseException<T>
    {
        public BloxlinkUserNotFoundException(BloxlinkResponse<T> response) : base(response) { }
    }
}
