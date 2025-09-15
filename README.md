# Bloxlink.Net

**Bloxlink.Net** is an unofficial **.NET API wrapper** for the [Bloxlink API](https://blox.link/developers). It provides strongly-typed clients for both **guild-specific** and **global** endpoints, making it easy to integrate Roblox–Discord verification into .NET applications such as bots, web services, and administrative tools.

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)

## ✨ Features

- 📡 **Guild Client** – Query Roblox accounts linked to Discord users *within a specific guild*.  
- 🌍 **Global Client** – Query Roblox accounts linked to Discord users *across all guilds*.  
- ⚡ **Async-first API** – Built on `async/await` with `HttpClient`.  
- 🗃️ **Caching built-in** – Results cached for 5 minutes by default, configurable.  
- 🔐 **Per-guild authorization** – Support for single or multiple guild API keys.  
- 📦 **NuGet distribution** – Simple installation via the public NuGet package.  

## 📦 Installing Bloxlink.Net

Stable builds are available on [NuGet](https://www.nuget.org/).

### Guild Client

The `BloxlinkGuildClient` utilizes the [Bloxlink Server API](https://blox.link/dashboard/user/developer) which can access individually bound accounts to a single Discord Guild.

```cs
// Create a client for a single guild with your API key
using var guildClient = new BloxlinkGuildClient("api-key", 899334250262822944);
```

You can also initialize the client with a dictionary of guild IDs → authorization keys. This is useful if your application operates across multiple servers with guild API keys.

```cs
// Dictionary where the key is the Guild ID and the value is the API key
var guildKeys = new Dictionary<ulong, string>
{
    { 899334250262822944, "api-key-1" },
    { 812345678901234567, "api-key-2" },
};

using var guildClient = new BloxlinkGuildClient(guildKeys);
```

The client will automatically select the correct key for the guild when making requests.

#### Retrieve a Roblox account linked to a Discord Guild Member.
```cs
ulong discordUserId = 123456789101112;
ulong guildId = 899334250262822944;

var robloxUser = await guildClient.GuildMemberToRoblox(discordUserId, guildId);
Console.WriteLine($"Roblox account ID: {robloxUser.AccountID}");
```

#### Fetch the Discord account linked to a Roblox user in a Discord Guild.
```cs
ulong guildId = 899334250262822944;
ulong robloxUserId = 248624943;

var guildMember = await client.RobloxToGuildMember(robloxUserId, guildId);
Console.WriteLine($"Discord account ID: {guildMember.AccountID}");
```

### Global Client

The `BloxlinkGlobalClient` utilizes the [Bloxlink Global API](https://blox.link/dashboard/user/developer) which can access global bound accounts that aren't tied to a specific Discord Guild.

```cs
// Create a global client with your API key
using var globalClient = new BloxlinkGlobalClient("api-key");
```

#### Get the Roblox account linked to a Discord user globally
```cs
ulong discordUserId = 123456789101112;

var robloxUser = await globalClient.DiscordToRobloxUser(discordUserId);
Console.WriteLine($"Roblox account ID: {robloxUser.AccountId}");

```

#### Get the Discord account linked to a Roblox user globally
```cs
ulong robloxUserId = 248624943;

var discordUser = await globalClient.RobloxToDiscordUser(robloxUserId);
Console.WriteLine($"Discord account ID: {discordUser.AccountId}");
```

## Account Information

Acccount objects only expose account IDs (Discord or Roblox). Future versions may provide richer information via `BloxlinkResolvedRobloxUser` when premium API features are documented and tested.

## Built-in Caching

* All retrievals are cached by default for 5 minutes `guildClient.CacheDuration`.
* Cache usage can be overridden per request by passing a `BloxlinkRequestOptions` instance.
* Cached values automatically expire based on the configured duration.

## Versioning

This library generally abides by [Semantic Versioning](https://semver.org). 
Packages are published in `BLOXLINK.MAJOR.MINOR.PATCH` version format.

An increment of the `BLOXLINK` component indicates that a new version of the [Bloxlink API](https://blox.link/developers) is supported.

All other increments of component follow what was described in the [Semantic Versioning Summary](https://semver.org/#summary).
