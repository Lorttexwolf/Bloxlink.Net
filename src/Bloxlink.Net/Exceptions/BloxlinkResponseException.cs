using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.Exceptions
{
    public class BloxlinkResponseException<T> : Exception
    {
        /// <summary>
        /// The <see cref="BloxlinkResponse{T}"></see> which was involved in the exception.
        /// </summary>
        public BloxlinkResponse<T> Response { get; }

        public BloxlinkResponseException(BloxlinkResponse<T> response) : base(response.ErrorReasoning ?? "Unable to reach Bloxlink")
        {
            this.Response = response;
        }
    }
}
