// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// A wrapper for the response Set-Cookie header.
/// </summary>
internal sealed partial class ResponseCookies : IResponseCookies
{
    internal const string EnableCookieNameEncoding = "Microsoft.AspNetCore.Http.EnableCookieNameEncoding";
    internal bool _enableCookieNameEncoding = AppContext.TryGetSwitch(EnableCookieNameEncoding, out var enabled) && enabled;

    private readonly IFeatureCollection _features;
    private ILogger? _logger;

    /// <summary>
    /// Create a new wrapper.
    /// </summary>
    internal ResponseCookies(IFeatureCollection features)
    {
        _features = features;
        Headers = _features.GetRequiredFeature<IHttpResponseFeature>().Headers;
    }

    private IHeaderDictionary Headers { get; set; }

    /// <inheritdoc />
    public void Append(string key, string value)
    {
        var setCookieHeaderValue = new SetCookieHeaderValue(
            _enableCookieNameEncoding ? Uri.EscapeDataString(key) : key,
            Uri.EscapeDataString(value))
        {
            Path = "/"
        };
        var cookieValue = setCookieHeaderValue.ToString();

        Headers.SetCookie = StringValues.Concat(Headers.SetCookie, cookieValue);
    }

    /// <inheritdoc />
    public void Append(string key, string value, CookieOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // SameSite=None cookies must be marked as Secure.
        if (!options.Secure && options.SameSite == SameSiteMode.None)
        {
            if (_logger == null)
            {
                var services = _features.Get<IServiceProvidersFeature>()?.RequestServices;
                _logger = services?.GetService<ILogger<ResponseCookies>>();
            }

            if (_logger != null)
            {
                Log.SameSiteCookieNotSecure(_logger, key);
            }
        }

        var cookie = options.CreateCookieHeader(
            _enableCookieNameEncoding ? Uri.EscapeDataString(key) : key,
            Uri.EscapeDataString(value)).ToString();

        Headers.SetCookie = StringValues.Concat(Headers.SetCookie, cookie);
    }

    /// <inheritdoc />
    public void Append(ReadOnlySpan<KeyValuePair<string, string>> keyValuePairs, CookieOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // SameSite=None cookies must be marked as Secure.
        if (!options.Secure && options.SameSite == SameSiteMode.None)
        {
            if (_logger == null)
            {
                var services = _features.Get<IServiceProvidersFeature>()?.RequestServices;
                _logger = services?.GetService<ILogger<ResponseCookies>>();
            }

            if (_logger != null)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    Log.SameSiteCookieNotSecure(_logger, keyValuePair.Key);
                }
            }
        }

        var cookieSuffix = options.CreateCookieHeader(string.Empty, string.Empty).ToString()[1..];
        var cookies = new string[keyValuePairs.Length];
        var position = 0;

        foreach (var keyValuePair in keyValuePairs)
        {
            var key = _enableCookieNameEncoding ? Uri.EscapeDataString(keyValuePair.Key) : keyValuePair.Key;
            cookies[position] = string.Concat(key, "=", Uri.EscapeDataString(keyValuePair.Value), cookieSuffix);
            position++;
        }

        // Can't use += as StringValues does not override operator+
        // and the implict conversions will cause an incorrect string concat https://github.com/dotnet/runtime/issues/52507
        Headers.SetCookie = StringValues.Concat(Headers.SetCookie, cookies);
    }

    /// <inheritdoc />
    public void Delete(string key)
    {
        Delete(key, new CookieOptions() { Path = "/" });
    }

    /// <inheritdoc />
    public void Delete(string key, CookieOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var encodedKeyPlusEquals = (_enableCookieNameEncoding ? Uri.EscapeDataString(key) : key) + "=";
        var domainHasValue = !string.IsNullOrEmpty(options.Domain);
        var pathHasValue = !string.IsNullOrEmpty(options.Path);

        Func<string, string, CookieOptions, bool> rejectPredicate;
        if (domainHasValue && pathHasValue)
        {
            rejectPredicate = (value, encKeyPlusEquals, opts) =>
                value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase) &&
                    value.IndexOf($"domain={opts.Domain}", StringComparison.OrdinalIgnoreCase) != -1 &&
                    value.IndexOf($"path={opts.Path}", StringComparison.OrdinalIgnoreCase) != -1;
        }
        else if (domainHasValue)
        {
            rejectPredicate = (value, encKeyPlusEquals, opts) =>
                value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase) &&
                    value.IndexOf($"domain={opts.Domain}", StringComparison.OrdinalIgnoreCase) != -1;
        }
        else if (pathHasValue)
        {
            rejectPredicate = (value, encKeyPlusEquals, opts) =>
                value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase) &&
                    value.IndexOf($"path={opts.Path}", StringComparison.OrdinalIgnoreCase) != -1;
        }
        else
        {
            rejectPredicate = (value, encKeyPlusEquals, opts) => value.StartsWith(encKeyPlusEquals, StringComparison.OrdinalIgnoreCase);
        }

        var existingValues = Headers.SetCookie;
        if (!StringValues.IsNullOrEmpty(existingValues))
        {
            var values = existingValues.ToArray();
            var newValues = new List<string>();

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i] ?? string.Empty;
                if (!rejectPredicate(value, encodedKeyPlusEquals, options))
                {
                    newValues.Add(value);
                }
            }

            Headers.SetCookie = new StringValues(newValues.ToArray());
        }

        Append(key, string.Empty, new CookieOptions(options)
        {
            Expires = DateTimeOffset.UnixEpoch,
        });
    }

    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Warning, "The cookie '{name}' has set 'SameSite=None' and must also set 'Secure'.", EventName = "SameSiteNotSecure")]
        public static partial void SameSiteCookieNotSecure(ILogger logger, string name);
    }
}
