using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bloxlink;
using System.Threading.Tasks;

namespace Bloxlink.Tests
{
    [TestClass]
    public class UserEndpointTest
    {
        [TestMethod]
        public async Task GetUser()
        {
            using var client = new BloxlinkClient();

            var user = await client.GetUser(84117866944663552);
        }

        [TestMethod]
        public async Task GetGuildUser()
        {
            using var client = new BloxlinkClient();

            var user = await client.GetUser(474413050594721802, 372036754078826496);

            Assert.IsNotNull(user);
        }
    }
}
