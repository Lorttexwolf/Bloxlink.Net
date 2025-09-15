using Bloxlink.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Bloxlink.Net.Tests
{
    [TestClass]
    public class GlobalClientTests
    {
        public BloxlinkGlobalClient Client { get; }

        public GlobalClientTests()
        {
            string authorizationKey = Utilities.RequireEnvironmentVariable("BLOXLINKNET_TEST_GLOBAL_CLIENT_KEY");

            this.Client = new BloxlinkGlobalClient(authorizationKey);
        }

        [TestMethod]
        public async Task DiscordToRobloxUser()
        {
            _ = await this.Client.DiscordToRobloxUser(474413050594721802);
        }

        //[TestMethod]
        //public async Task RobloxToDiscordUser()
        //{
        //    // TODO: Obtain privileged access for unit testing.
        //    //_ = await this.Client.RobloxToDiscordUser(248624943);
        //}
    }
}
