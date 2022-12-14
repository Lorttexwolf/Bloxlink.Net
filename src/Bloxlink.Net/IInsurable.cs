using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink
{
    public interface IInsurable
    {
        void EnsureSuccess();

        public static void EnsureSuccess<T>(T insurable) where T : IInsurable => insurable.EnsureSuccess();
    }
}
