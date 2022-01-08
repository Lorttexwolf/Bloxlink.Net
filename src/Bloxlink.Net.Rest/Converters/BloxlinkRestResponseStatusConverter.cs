using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bloxlink.Rest
{
    // TODO: Expand on exception message 
    public class BloxlinkRestResponseStatusConverter : JsonConverter<BloxlinkRestResponse.StatusType>
    {
        public override BloxlinkRestResponse.StatusType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() switch
            {
                "ok" => BloxlinkRestResponse.StatusType.Ok,
                "error" => BloxlinkRestResponse.StatusType.Error,
                _ => throw new InvalidOperationException("Failed to read BloxlinkRestResponse status")
            };
        }

        public override void Write(Utf8JsonWriter writer, BloxlinkRestResponse.StatusType value, JsonSerializerOptions options)
        {
            string valueToWrite = value switch
            {
                BloxlinkRestResponse.StatusType.Ok => "ok",
                BloxlinkRestResponse.StatusType.Error => "error",
                _ => throw new NotImplementedException("Failed to write BloxlinkRestResponse status")
            };
            writer.WriteString("status", valueToWrite);
        }
    }
}
