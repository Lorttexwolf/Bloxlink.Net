using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;

namespace Bloxlink.Tests
{
    [TestClass]
    public class UserEndpointTest
    {
        [TestMethod("Key-Validation")]
        public void KeyValidation()
        {
            Assert.ThrowsExceptionAsync<AggregateException>(async () =>
            {
                using var client = new BloxlinkClient("00000000-0000-0000-0000-000000000000");
                await client.ValidateKey();
            });
        }

        [TestMethod]
        public async Task GetUser()
        {
            using var client = new BloxlinkClient("api-key");
            await client.ValidateKey();

            var user = await client.GetUserAsync(474413050594721802);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task GetGuildUser()
        {
            using var client = new BloxlinkClient("api-key");
            await client.ValidateKey();

            var user = await client.GetUserAsync(474413050594721802, 372036754078826496);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task GetCachedUser()
        {
            using var client = new BloxlinkClient("api-key");
            await client.ValidateKey();

            var req = await client.GetUserAsync(474413050594721802, 372036754078826496);
            Assert.IsNotNull(req);

            Console.WriteLine($"Fetched {req.User.GuildAccount}");

            var cachedUser = client.GetUser(474413050594721802, 372036754078826496);
            Assert.AreEqual(cachedUser.Value, req.User.GuildAccount);
        }

        /*[TestMethod]
        public async Task GetUserWithRatelimit()
        {
            using var client = new BloxlinkClient("api-key");

            for (var i = 0; i < 20; i++)
            {
                var req = await client.GetUserAsync(84117866944663552, options: new BloxlinkRequestOptions()
                {
                    RetryOnRatelimit = false,
                    PopulateCache = false
                });
                Console.WriteLine(req.User.GlobalAccount);
                Assert.IsNotNull(req);
            }
        }*/
    }
}
