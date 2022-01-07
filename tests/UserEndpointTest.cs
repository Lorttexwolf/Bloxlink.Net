using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bloxlink;
using System.Threading.Tasks;
using System;

namespace Bloxlink.Tests
{
    [TestClass]
    public class UserEndpointTest
    {
        [TestMethod]
        public async Task GetUser()
        {
            using var client = new BloxlinkClient();

            var user = await client.GetUserAsync(84117866944663552);
        }

        [TestMethod]
        public async Task GetGuildUser()
        {
            using var client = new BloxlinkClient();

            var user = await client.GetUserAsync(474413050594721802, 372036754078826496);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task GetUserWithRatelimit()
        {
            using var client = new BloxlinkClient();

            // Exceed rate limits to test RetryOnRatelimit
            for (var i = 0; i < 80; i++)
            {
                // Do not use the cache during this test.
                var user = await client.GetUserAsync(84117866944663552, false, options: new Rest.BloxlinkRestRequestOptions()
                {
                    RetryOnRatelimit = true
                });

                Assert.IsNotNull(user);
            }
        }
    }
}
