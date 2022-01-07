using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bloxlink.Rest
{
    public static class UriBuilderExtensions
    {
        public static void AddQueryParameter(this UriBuilder builder, string name, string? value)
        {
            var query = HttpUtility.ParseQueryString(builder.Query ?? "");
            query.Set(name, value);

            builder.Query = query.ToString();
        }
    }
}
