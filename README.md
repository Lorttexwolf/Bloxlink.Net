# Bloxlink.Net
Bloxlink.Net is an unofficial .NET API Wrapper for the [Bloxlink API](https://blox.link/developers).

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/you-didnt-ask-for-this.svg)](https://forthebadge.com)

### **Installing Bloxlink.Net** 📦
> Stable builds are available on [NuGet](https://www.nuget.org/) through the [Bloxlink Nuget Package](https://www.nuget.org/packages/Bloxlink.Net/).

### Surfing the Search API 🔍
> The Search API allows you to determine what Robox accounts are connected to a Discord user.
> 
> **Note**:
> This is NOT the Roblox to Discord API.
```cs
using var client = new BloxlinkClient("api-key");
await client.ValidateKey(); // Make sure to validate your key!

// Get the primary account.
ulong discordUserId = 123456789101112;
var req = await client.GetUserAsync(discordUserId);
Console.WriteLine($"Fetched: {req.User.GlobalAccount}");

// Get the account linked to a guild.
ulong guildId = 372036754078826496;
req = await client.GetUserAsync(discordUserId, guildId);
Console.WriteLine($"Fetched: {req.User.GuildAccount}");
```

### Utilizing the Built-in Cache ⚙
> Retrieved users are cached by default, you can access them using the `GetUser` method.
> 
> **Note**:
> The cache is only cleared when the `BloxlinkClient` is disposed.
```cs
var res = await client.GetUserAsync(123456789101112, options: new() { PopulateCache = true });
Console.WriteLine($"Fetched: {res.User.GlobalAccount}");

// You may access your remaining quota in the BloxlinkResponse.
Console.WriteLine($"Quota Remaining: {res.QuotaRemaining}");

var cachedUserId = client.GetUser(123456789101112)!;
Console.WriteLine($"Cached user: {cachedUserId}");
```

### Exception Handling 🚧
> Several custom-exceptions such as `UserNotFound` and `QuotaExceeded` are provided for ease-of-use!
```cs
try
{
    var res = await client.GetUserAsync(69552131231221232);
}
catch (UserNotFoundException)
{
	Console.WriteLine("User was not found.");
}
catch (QuotaExceededException)
{
    Console.WriteLine("We have exceeded our quota!");
}
```

## Versioning Guarantees
This library generally abides by [Semantic Versioning](https://semver.org). Packages are published in MAJOR.MINOR.PATCH version format.

An increment of the MAJOR component indicates that a new version of the [Bloxlink API](https://blox.link/developers) is supported.

All other increments of component follow what was described in the [Semantic Versioning Summary](https://semver.org/#summary).
