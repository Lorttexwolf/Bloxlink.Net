using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Bloxlink.Clients;

namespace Bloxlink.Net.Tests
{
    [TestClass]
    public class GuildClientTests
    {
        public BloxlinkGuildClient Client { get; }

        public GuildClientTests()
        {
            string authorizationKey = Utilities.RequireEnvironmentVariable("BLOXLINKNET_TEST_GUILD_CLIENT_KEY");

            this.Client = new BloxlinkGuildClient(authorizationKey, 899334250262822944);
        }

        [TestMethod]
        public async Task GuildMemberToRoblox()
        {
            _ = await this.Client.GuildMemberToRoblox(474413050594721802, 899334250262822944);
        }

        [TestMethod]
        public async Task SearchRobloxInDiscordGuild()
        {
            _ = await this.Client.RobloxToGuildMember(248624943, 899334250262822944);
        }
    }
}
