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
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException() : base("Bloxlink quota exceeded.") { }
    }
}
