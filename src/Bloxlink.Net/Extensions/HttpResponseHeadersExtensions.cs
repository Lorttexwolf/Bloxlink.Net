using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.Extensions
{
    internal static class HttpResponseHeadersExtensions
    {
        public static bool Has(this HttpResponseHeaders headers, string name)
            => headers.TryGetValues(name, out var _);
    }
}
