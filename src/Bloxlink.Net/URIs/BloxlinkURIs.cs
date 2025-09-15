using Bloxlink.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxlink.URIs
{
    internal class BloxlinkURIs
    {
        public const string BaseHTTPAddress = "https://api.blox.link/v4";

        public static string DiscordGuildToRoblox(ulong discordUserId, ulong guildId)
        {
            var builder = new UriBuilder($"{BaseHTTPAddress}/public/guilds/{guildId}/discord-to-roblox/{discordUserId}");

            builder.AddQueryParameter("guildId", guildId.ToString());

            return builder.Uri.ToString();
        }

        public static string RobloxToDiscordGuild(ulong robloxAccountId, ulong guildId)
        {
            return $"{BaseHTTPAddress}/public/guilds/{guildId}/roblox-to-discord/{robloxAccountId}";
        }

        public static string DiscordToRoblox(ulong discordUserId)
        {
            return $"{BaseHTTPAddress}/public/discord-to-roblox/{discordUserId}";
        }

        public static string RobloxToDiscord(ulong robloxAccountId)
        {
            return $"{BaseHTTPAddress}/public/roblox-to-discord/{robloxAccountId}";
        }
    }
}
