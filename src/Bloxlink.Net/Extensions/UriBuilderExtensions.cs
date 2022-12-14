using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bloxlink.Extensions
{
    internal static class UriBuilderExtensions
    {
        internal static void AddQueryParameter(this UriBuilder builder, string name, string? value)
        {
            var query = HttpUtility.ParseQueryString(builder.Query ?? "");
            query.Set(name, value);

            builder.Query = query.ToString();
        }
    }
}
