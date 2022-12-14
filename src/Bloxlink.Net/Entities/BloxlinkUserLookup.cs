namespace Bloxlink
{
    /// <summary>
    /// Represents a discord-to-roblox query for Bloxlink.
    /// </summary>
    public record BloxlinkUserLookup
    {
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }

        public BloxlinkUserLookup(ulong discordId, ulong? guildId = null)
        {
            this.DiscordId = discordId;
            this.GuildId = guildId ?? 0;
        }
    }
}
