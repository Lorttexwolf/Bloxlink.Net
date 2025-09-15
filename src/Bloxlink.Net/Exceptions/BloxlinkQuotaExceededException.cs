using Bloxlink.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink
{
    /// <summary>
    /// An <see cref="Exception"/> indicating that this key has exceeded its quota.
    /// </summary>
    public class BloxlinkQuotaExceededException<T> : BloxlinkResponseException<T>
    {
        public BloxlinkQuotaExceededException(BloxlinkResponse<T> response) : base(response) { }
    }
}
