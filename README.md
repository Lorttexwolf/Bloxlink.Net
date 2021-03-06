# Bloxlink.Net
Bloxlink.Net is an unofficial .NET API Wrapper for the [Bloxlink API](https://blox.link/developers)

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/you-didnt-ask-for-this.svg)](https://forthebadge.com)

## Installation (NuGet)
Stable builds are available from [NuGet](https://www.nuget.org/) through the [Bloxlink Nuget Package](https://www.nuget.org/packages/Bloxlink.Net/)


## Examples
___
### Getting a linked Roblox account using a Discord account
```cs
using var client = new BloxlinkClient();

ulong discordUserId = 123456789101112;
var robloxUserId = await client.GetUserAsync(discordUserId, cache: true);
// Their Roblox account that's linked to a certain guild, not globally.
// var robloxUserId = await client.GetUserAsync(discordUserId, guildId);
```

## Versioning Guarantees
___
This library generally abides by [Semantic Versioning](https://semver.org). Packages are published in MAJOR.MINOR.PATCH version format.

An increment of the MAJOR component indicates that a new version of the [Bloxlink API](https://blox.link/developers) is supported.

All other increments of component follow what was described in the [Semantic Versioning Summary](https://semver.org/#summary).
