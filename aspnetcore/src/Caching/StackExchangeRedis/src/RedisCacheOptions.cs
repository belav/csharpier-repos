// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.Configuration;
using StackExchange.Redis.Profiling;

namespace Microsoft.Extensions.Caching.StackExchangeRedis;

/// <summary>
/// Configuration options for <see cref="RedisCache"/>.
/// </summary>
public class RedisCacheOptions : IOptions<RedisCacheOptions>
{
    /// <summary>
    /// The configuration used to connect to Redis.
    /// </summary>
    public string? Configuration { get; set; }

    /// <summary>
    /// The configuration used to connect to Redis.
    /// This is preferred over Configuration.
    /// </summary>
    public ConfigurationOptions? ConfigurationOptions { get; set; }

    /// <summary>
    /// Gets or sets a delegate to create the ConnectionMultiplexer instance.
    /// </summary>
    public Func<Task<IConnectionMultiplexer>>? ConnectionMultiplexerFactory { get; set; }

    /// <summary>
    /// The Redis instance name. Allows partitioning a single backend cache for use with multiple apps/services.
    /// If set, the cache keys are prefixed with this value.
    /// </summary>
    public string? InstanceName { get; set; }

    /// <summary>
    /// The Redis profiling session
    /// </summary>
    public Func<ProfilingSession>? ProfilingSession { get; set; }

    RedisCacheOptions IOptions<RedisCacheOptions>.Value
    {
        get { return this; }
    }

    private bool? _useForceReconnect;
    internal bool UseForceReconnect
    {
        get
        {
            return _useForceReconnect ??= GetDefaultValue();
            static bool GetDefaultValue() =>
                AppContext.TryGetSwitch(
                    "Microsoft.AspNetCore.Caching.StackExchangeRedis.UseForceReconnect",
                    out var value
                ) && value;
        }
        set => _useForceReconnect = value;
    }

    internal ConfigurationOptions GetConfiguredOptions(string libSuffix)
    {
        var options = ConfigurationOptions?.Clone() ?? ConfigurationOptions.Parse(Configuration!);

        // we don't want an initially unavailable server to prevent DI creating the service itself
        options.AbortOnConnectFail = false;

        if (!string.IsNullOrWhiteSpace(libSuffix))
        {
            var provider = DefaultOptionsProvider.GetProvider(options.EndPoints);
            options.LibraryName = $"{provider.LibraryName} {libSuffix}";
        }
        return options;
    }
}
