// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Microsoft.Extensions.Caching.StackExchangeRedis;

/// <summary>
/// Distributed cache implementation using Redis.
/// <para>Uses <c>StackExchange.Redis</c> as the Redis client.</para>
/// </summary>
public partial class RedisCache : IDistributedCache, IDisposable
{
    // -- Explanation of why two kinds of SetScript are used --
    // * Redis 2.0 had HSET key field value for setting individual hash fields,
    // and HMSET key field value [field value ...] for setting multiple hash fields (against the same key).
    // * Redis 4.0 added HSET key field value [field value ...] and deprecated HMSET.
    //
    // On Redis versions that don't have the newer HSET variant, we use SetScriptPreExtendedSetCommand
    // which uses the (now deprecated) HMSET.

    // KEYS[1] = = key
    // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
    // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
    // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
    // ARGV[4] = data - byte[]
    // this order should not change LUA script depends on it
    private const string SetScript = (@"
                redis.call('HSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1");
    private const string SetScriptPreExtendedSetCommand = (@"
                redis.call('HMSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1");

    private const string AbsoluteExpirationKey = "absexp";
    private const string SlidingExpirationKey = "sldexp";
    private const string DataKey = "data";
    private const long NotPresent = -1;
    private static readonly Version ServerVersionWithExtendedSetCommand = new Version(4, 0, 0);

    private volatile IConnectionMultiplexer? _connection;
    private IDatabase? _cache;
    private bool _disposed;
    private string _setScript = SetScript;

    private readonly RedisCacheOptions _options;
    private readonly string _instance;
    private readonly ILogger _logger;

    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

    /// <summary>
    /// Initializes a new instance of <see cref="RedisCache"/>.
    /// </summary>
    /// <param name="optionsAccessor">The configuration options.</param>
    public RedisCache(IOptions<RedisCacheOptions> optionsAccessor)
        : this(optionsAccessor, Logging.Abstractions.NullLoggerFactory.Instance.CreateLogger<RedisCache>())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RedisCache"/>.
    /// </summary>
    /// <param name="optionsAccessor">The configuration options.</param>
    /// <param name="logger">The logger.</param>
    internal RedisCache(IOptions<RedisCacheOptions> optionsAccessor, ILogger logger)
    {
        if (optionsAccessor == null)
        {
            throw new ArgumentNullException(nameof(optionsAccessor));
        }

        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _options = optionsAccessor.Value;
        _logger = logger;

        // This allows partitioning a single backend cache for use with multiple apps/services.
        _instance = _options.InstanceName ?? string.Empty;
    }

    /// <inheritdoc />
    public byte[]? Get(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return GetAndRefresh(key, getData: true);
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        token.ThrowIfCancellationRequested();

        return await GetAndRefreshAsync(key, getData: true, token: token).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        Connect();

        var creationTime = DateTimeOffset.UtcNow;

        var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

        _cache.ScriptEvaluate(_setScript, new RedisKey[] { _instance + key },
            new RedisValue[]
            {
                        absoluteExpiration?.Ticks ?? NotPresent,
                        options.SlidingExpiration?.Ticks ?? NotPresent,
                        GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                        value
            });
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        token.ThrowIfCancellationRequested();

        await ConnectAsync(token).ConfigureAwait(false);
        Debug.Assert(_cache is not null);

        var creationTime = DateTimeOffset.UtcNow;

        var absoluteExpiration = GetAbsoluteExpiration(creationTime, options);

        await _cache.ScriptEvaluateAsync(_setScript, new RedisKey[] { _instance + key },
            new RedisValue[]
            {
                absoluteExpiration?.Ticks ?? NotPresent,
                options.SlidingExpiration?.Ticks ?? NotPresent,
                GetExpirationInSeconds(creationTime, absoluteExpiration, options) ?? NotPresent,
                value
            }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Refresh(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        GetAndRefresh(key, getData: false);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        token.ThrowIfCancellationRequested();

        await GetAndRefreshAsync(key, getData: false, token: token).ConfigureAwait(false);
    }

    [MemberNotNull(nameof(_cache), nameof(_connection))]
    private void Connect()
    {
        CheckDisposed();
        if (_cache != null)
        {
            Debug.Assert(_connection != null);
            return;
        }

        _connectionLock.Wait();
        try
        {
            if (_cache == null)
            {
                if (_options.ConnectionMultiplexerFactory == null)
                {
                    if (_options.ConfigurationOptions is not null)
                    {
                        _connection = ConnectionMultiplexer.Connect(_options.ConfigurationOptions);
                    }
                    else
                    {
                        _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                    }
                }
                else
                {
                    _connection = _options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();
                }

                PrepareConnection();
                _cache = _connection.GetDatabase();
            }
        }
        finally
        {
            _connectionLock.Release();
        }

        Debug.Assert(_connection != null);
    }

    private async Task ConnectAsync(CancellationToken token = default(CancellationToken))
    {
        CheckDisposed();
        token.ThrowIfCancellationRequested();

        if (_cache != null)
        {
            Debug.Assert(_connection != null);
            return;
        }

        await _connectionLock.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (_cache == null)
            {
                if (_options.ConnectionMultiplexerFactory is null)
                {
                    if (_options.ConfigurationOptions is not null)
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions).ConfigureAwait(false);
                    }
                    else
                    {
                        _connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration).ConfigureAwait(false);
                    }
                }
                else
                {
                    _connection = await _options.ConnectionMultiplexerFactory().ConfigureAwait(false);
                }

                PrepareConnection();
                _cache = _connection.GetDatabase();
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private void PrepareConnection()
    {
        ValidateServerFeatures();
        TryRegisterProfiler();
    }

    private void ValidateServerFeatures()
    {
        _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

        try
        {
            foreach (var endPoint in _connection.GetEndPoints())
            {
                if (_connection.GetServer(endPoint).Version < ServerVersionWithExtendedSetCommand)
                {
                    _setScript = SetScriptPreExtendedSetCommand;
                    return;
                }
            }
        }
        catch (NotSupportedException ex)
        {
            Log.CouldNotDetermineServerVersion(_logger, ex);

            // The GetServer call may not be supported with some configurations, in which
            // case let's also fall back to using the older command.
            _setScript = SetScriptPreExtendedSetCommand;
        }
    }

    private void TryRegisterProfiler()
    {
        _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

        if (_options.ProfilingSession != null)
        {
            _connection.RegisterProfiler(_options.ProfilingSession);
        }
    }

    private byte[]? GetAndRefresh(string key, bool getData)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        Connect();

        // This also resets the LRU status as desired.
        // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
        RedisValue[] results;
        if (getData)
        {
            results = _cache.HashMemberGet(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey);
        }
        else
        {
            results = _cache.HashMemberGet(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey);
        }

        // TODO: Error handling
        if (results.Length >= 2)
        {
            MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
            Refresh(_cache, key, absExpr, sldExpr);
        }

        if (results.Length >= 3 && results[2].HasValue)
        {
            return results[2];
        }

        return null;
    }

    private async Task<byte[]?> GetAndRefreshAsync(string key, bool getData, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        token.ThrowIfCancellationRequested();

        await ConnectAsync(token).ConfigureAwait(false);
        Debug.Assert(_cache is not null);

        // This also resets the LRU status as desired.
        // TODO: Can this be done in one operation on the server side? Probably, the trick would just be the DateTimeOffset math.
        RedisValue[] results;
        if (getData)
        {
            results = await _cache.HashMemberGetAsync(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey, DataKey).ConfigureAwait(false);
        }
        else
        {
            results = await _cache.HashMemberGetAsync(_instance + key, AbsoluteExpirationKey, SlidingExpirationKey).ConfigureAwait(false);
        }

        // TODO: Error handling
        if (results.Length >= 2)
        {
            MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
            await RefreshAsync(_cache, key, absExpr, sldExpr, token).ConfigureAwait(false);
        }

        if (results.Length >= 3 && results[2].HasValue)
        {
            return results[2];
        }

        return null;
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        Connect();

        _cache.KeyDelete(_instance + key);
        // TODO: Error handling
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        await ConnectAsync(token).ConfigureAwait(false);
        Debug.Assert(_cache is not null);

        await _cache.KeyDeleteAsync(_instance + key).ConfigureAwait(false);
        // TODO: Error handling
    }

    private static void MapMetadata(RedisValue[] results, out DateTimeOffset? absoluteExpiration, out TimeSpan? slidingExpiration)
    {
        absoluteExpiration = null;
        slidingExpiration = null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != NotPresent)
        {
            absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);
        }
        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != NotPresent)
        {
            slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
        }
    }

    private void Refresh(IDatabase cache, string key, DateTimeOffset? absExpr, TimeSpan? sldExpr)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        // Note Refresh has no effect if there is just an absolute expiration (or neither).
        if (sldExpr.HasValue)
        {
            TimeSpan? expr;
            if (absExpr.HasValue)
            {
                var relExpr = absExpr.Value - DateTimeOffset.Now;
                expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
            }
            else
            {
                expr = sldExpr;
            }
            cache.KeyExpire(_instance + key, expr);
            // TODO: Error handling
        }
    }

    private async Task RefreshAsync(IDatabase cache, string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, CancellationToken token = default(CancellationToken))
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        token.ThrowIfCancellationRequested();

        // Note Refresh has no effect if there is just an absolute expiration (or neither).
        if (sldExpr.HasValue)
        {
            TimeSpan? expr;
            if (absExpr.HasValue)
            {
                var relExpr = absExpr.Value - DateTimeOffset.Now;
                expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
            }
            else
            {
                expr = sldExpr;
            }
            await cache.KeyExpireAsync(_instance + key, expr).ConfigureAwait(false);
            // TODO: Error handling
        }
    }

    private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
    {
        if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
        {
            return (long)Math.Min(
                (absoluteExpiration.Value - creationTime).TotalSeconds,
                options.SlidingExpiration.Value.TotalSeconds);
        }
        else if (absoluteExpiration.HasValue)
        {
            return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
        }
        else if (options.SlidingExpiration.HasValue)
        {
            return (long)options.SlidingExpiration.Value.TotalSeconds;
        }
        return null;
    }

    private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
    {
        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
        {
            throw new ArgumentOutOfRangeException(
                nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");
        }

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            return creationTime + options.AbsoluteExpirationRelativeToNow;
        }

        return options.AbsoluteExpiration;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _connection?.Close();
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }
    }
}
