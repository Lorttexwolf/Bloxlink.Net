using System;

namespace Bloxlink.Net.Tests
{
    internal class Utilities
    {
        public static string RequireEnvironmentVariable(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);

            return value ?? throw new InvalidOperationException($"Environment variable {name} must be provided!");
        }
    }
}
