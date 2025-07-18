// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http.Headers;

/// <summary>
/// Strongly typed HTTP response headers.
/// </summary>
public class ResponseHeaders
{
    /// <summary>
    /// Initializes a new instance of <see cref="ResponseHeaders"/>.
    /// </summary>
    /// <param name="headers">The request headers.</param>
    public ResponseHeaders(IHeaderDictionary headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        Headers = headers;
    }

    /// <summary>
    /// Gets the backing response header dictionary.
    /// </summary>
    public IHeaderDictionary Headers { get; }

    /// <summary>
    /// Gets or sets the <c>Cache-Control</c> header for an HTTP response.
    /// </summary>
    public CacheControlHeaderValue? CacheControl
    {
        get { return Headers.Get<CacheControlHeaderValue>(HeaderNames.CacheControl); }
        set { Headers.Set(HeaderNames.CacheControl, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Content-Disposition</c> header for an HTTP response.
    /// </summary>
    public ContentDispositionHeaderValue? ContentDisposition
    {
        get { return Headers.Get<ContentDispositionHeaderValue>(HeaderNames.ContentDisposition); }
        set { Headers.Set(HeaderNames.ContentDisposition, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Content-Length</c> header for an HTTP response.
    /// </summary>
    public long? ContentLength
    {
        get { return Headers.ContentLength; }
        set { Headers.ContentLength = value; }
    }

    /// <summary>
    /// Gets or sets the <c>Content-Range</c> header for an HTTP response.
    /// </summary>
    public ContentRangeHeaderValue? ContentRange
    {
        get { return Headers.Get<ContentRangeHeaderValue>(HeaderNames.ContentRange); }
        set { Headers.Set(HeaderNames.ContentRange, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Content-Type</c> header for an HTTP response.
    /// </summary>
    public MediaTypeHeaderValue? ContentType
    {
        get { return Headers.Get<MediaTypeHeaderValue>(HeaderNames.ContentType); }
        set { Headers.Set(HeaderNames.ContentType, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Date</c> header for an HTTP response.
    /// </summary>
    public DateTimeOffset? Date
    {
        get { return Headers.GetDate(HeaderNames.Date); }
        set { Headers.SetDate(HeaderNames.Date, value); }
    }

    /// <summary>
    /// Gets or sets the <c>ETag</c> header for an HTTP response.
    /// </summary>
    public EntityTagHeaderValue? ETag
    {
        get { return Headers.Get<EntityTagHeaderValue>(HeaderNames.ETag); }
        set { Headers.Set(HeaderNames.ETag, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Expires</c> header for an HTTP response.
    /// </summary>
    public DateTimeOffset? Expires
    {
        get { return Headers.GetDate(HeaderNames.Expires); }
        set { Headers.SetDate(HeaderNames.Expires, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Last-Modified</c> header for an HTTP response.
    /// </summary>
    public DateTimeOffset? LastModified
    {
        get { return Headers.GetDate(HeaderNames.LastModified); }
        set { Headers.SetDate(HeaderNames.LastModified, value); }
    }

    /// <summary>
    /// Gets or sets the <c>Location</c> header for an HTTP response.
    /// </summary>
    public Uri? Location
    {
        get
        {
            if (Uri.TryCreate(Headers.Location, UriKind.RelativeOrAbsolute, out var uri))
            {
                return uri;
            }
            return null;
        }
        set { Headers.Set(HeaderNames.Location, value == null ? null : UriHelper.Encode(value)); }
    }

    /// <summary>
    /// Gets or sets the <c>Set-Cookie</c> header for an HTTP response.
    /// </summary>
    public IList<SetCookieHeaderValue> SetCookie
    {
        get { return Headers.SetCookie.GetList<SetCookieHeaderValue>(); }
        set { Headers.SetList(HeaderNames.SetCookie, value); }
    }

    /// <summary>
    /// Gets the value of header with <paramref name="name"/>.
    /// </summary>
    /// <remarks><typeparamref name="T"/> must contain a TryParse method with the signature <c>public static bool TryParse(string, out T)</c>.</remarks>
    /// <typeparam name="T">The type of the header.
    /// The given type must have a static TryParse method.</typeparam>
    /// <param name="name">The name of the header to retrieve.</param>
    /// <returns>The value of the header.</returns>
    public T? Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        string name
    )
    {
        return Headers.Get<T>(name);
    }

    /// <summary>
    /// Gets the values of header with <paramref name="name"/>.
    /// </summary>
    /// <remarks><typeparamref name="T"/> must contain a TryParseList method with the signature <c>public static bool TryParseList(IList&lt;string&gt;, out IList&lt;T&gt;)</c>.</remarks>
    /// <typeparam name="T">The type of the header.
    /// The given type must have a static TryParseList method.</typeparam>
    /// <param name="name">The name of the header to retrieve.</param>
    /// <returns>List of values of the header.</returns>
    public IList<T> GetList<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T
    >(string name)
    {
        return Headers.GetList<T>(name);
    }

    /// <summary>
    /// Sets the header value.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    public void Set(string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(name);

        Headers.Set(name, value);
    }

    /// <summary>
    /// Sets the specified header and it's values.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="name">The header name.</param>
    /// <param name="values">The sequence of header values.</param>
    public void SetList<T>(string name, IList<T>? values)
    {
        ArgumentNullException.ThrowIfNull(name);

        Headers.SetList<T>(name, values);
    }

    /// <summary>
    /// Appends the header name and value.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    public void Append(string name, object value)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        Headers.Append(name, value.ToString());
    }

    /// <summary>
    /// Appends the header name and it's values.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="values">The header values.</param>
    public void AppendList<T>(string name, IList<T> values)
    {
        Headers.AppendList<T>(name, values);
    }
}
