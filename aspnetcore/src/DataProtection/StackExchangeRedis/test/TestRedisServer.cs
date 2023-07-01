using System;
using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.DataProtection;

internal class TestRedisServer
{
    public const string ConnectionStringKeyName = "Test:Redis:Server";
    private static readonly IConfigurationRoot _config;

    static TestRedisServer()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("testconfig.json")
            .AddEnvironmentVariables()
            .Build();
    }

    internal static string GetConnectionString()
    {
        return _config[ConnectionStringKeyName];
    }
}
