// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http.Abstractions;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Defines settings used to create a cookie.
/// </summary>
public class CookieBuilder
{
    private string? _name;
    private List<string>? _extensions;

    /// <summary>
    /// The name of the cookie.
    /// </summary>
    public virtual string? Name
    {
        get => _name;
        set =>
            _name = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(value));
    }

    /// <summary>
    /// The cookie path.
    /// </summary>
    /// <remarks>
    /// Determines the value that will be set for <see cref="CookieOptions.Path"/>.
    /// </remarks>
    public virtual string? Path { get; set; }

    /// <summary>
    /// The domain to associate the cookie with.
    /// </summary>
    /// <remarks>
    /// Determines the value that will be set for <see cref="CookieOptions.Domain"/>.
    /// </remarks>
    public virtual string? Domain { get; set; }

    /// <summary>
    /// Indicates whether a cookie is inaccessible by client-side script. The default value is <c>false</c>
    /// but specific components may use a different value.
    /// </summary>
    /// <remarks>
    /// Determines the value that will be set on <see cref="CookieOptions.HttpOnly"/>.
    /// </remarks>
    public virtual bool HttpOnly { get; set; }

    /// <summary>
    /// The SameSite attribute of the cookie. The default value is <see cref="SameSiteMode.Unspecified"/>
    /// but specific components may use a different value.
    /// </summary>
    /// <remarks>
    /// Determines the value that will be set for <see cref="CookieOptions.SameSite"/>.
    /// </remarks>
    public virtual SameSiteMode SameSite { get; set; } = SameSiteMode.Unspecified;

    /// <summary>
    /// The policy that will be used to determine <see cref="CookieOptions.Secure"/>.
    /// This is determined from the <see cref="HttpContext"/> passed to <see cref="Build(HttpContext, DateTimeOffset)"/>.
    /// </summary>
    public virtual CookieSecurePolicy SecurePolicy { get; set; }

    /// <summary>
    /// Gets or sets the lifespan of a cookie.
    /// </summary>
    public virtual TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Gets or sets the max-age for the cookie.
    /// </summary>
    public virtual TimeSpan? MaxAge { get; set; }

    /// <summary>
    /// Indicates if this cookie is essential for the application to function correctly. If true then
    /// consent policy checks may be bypassed. The default value is <c>false</c>
    /// but specific components may use a different value.
    /// </summary>
    public virtual bool IsEssential { get; set; }

    /// <summary>
    /// Gets a collection of additional values to append to the cookie.
    /// </summary>
    public IList<string> Extensions
    {
        get => _extensions ??= new List<string>();
    }

    /// <summary>
    /// Creates the cookie options from the given <paramref name="context"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>The cookie options.</returns>
    public CookieOptions Build(HttpContext context) => Build(context, DateTimeOffset.Now);

    /// <summary>
    /// Creates the cookie options from the given <paramref name="context"/> with an expiration based on <paramref name="expiresFrom"/> and <see cref="Expiration"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="expiresFrom">The time to use as the base for computing <see cref="CookieOptions.Expires" />.</param>
    /// <returns>The cookie options.</returns>
    public virtual CookieOptions Build(HttpContext context, DateTimeOffset expiresFrom)
    {
        ArgumentNullException.ThrowIfNull(context);

        var options = new CookieOptions
        {
            Path = Path ?? "/",
            SameSite = SameSite,
            HttpOnly = HttpOnly,
            MaxAge = MaxAge,
            Domain = Domain,
            IsEssential = IsEssential,
            Secure =
                SecurePolicy == CookieSecurePolicy.Always
                || (SecurePolicy == CookieSecurePolicy.SameAsRequest && context.Request.IsHttps),
            Expires = Expiration.HasValue
                ? expiresFrom.Add(Expiration.GetValueOrDefault())
                : default(DateTimeOffset?),
        };

        if (_extensions?.Count > 0)
        {
            foreach (var extension in _extensions)
            {
                options.Extensions.Add(extension);
            }
        }
        return options;
    }
}
