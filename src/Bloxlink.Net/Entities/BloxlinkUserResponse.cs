using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink
{
    public class BloxlinkUserResponse : BloxlinkResponse
    {
        [JsonPropertyName("user")]
        public BloxlinkUser User { get; set; }

        public override void EnsureSuccess()
        {
            base.EnsureSuccess();
            IInsurable.EnsureSuccess(this.User);
        }
    }
}
