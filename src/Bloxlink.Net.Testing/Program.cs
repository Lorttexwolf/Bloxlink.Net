using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bloxlink.Rest;

namespace Bloxlink.Testing
{
    internal class Program
    {
        protected readonly SemaphoreSlim _stateLock = new(1, 1);

        static async Task Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            /*var thingy = new RestWaiter();

            thingy.WaitAnother(new TimeSpan(0, 0, 5));

            thingy.Sleep();

            Console.WriteLine("Hey");

            thingy.WaitAnother(new TimeSpan(0, 0, 10));

            thingy.Sleep();

            Console.WriteLine("Yo");*/

            Console.WriteLine("Hello World!");

            using var bloxlinkClient = new BloxlinkRestClient();

            var tasks = new List<Task<BloxlinkRestUserResponse>>();
            
            for (int i = 0; i < 140; i++)
            {
                tasks.Add(bloxlinkClient.GetRobloxUser(474413050594721802, options: new BloxlinkRestRequestOptions()
                {
                    RetryAtTimeout = true
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("Hi");
        }
    }
}
