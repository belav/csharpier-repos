// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http.Headers
{
    /// <summary>
    /// Strongly typed HTTP request headers.
    /// </summary>
    public class RequestHeaders
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RequestHeaders"/>.
        /// </summary>
        /// <param name="headers">The request headers.</param>
        public RequestHeaders(IHeaderDictionary headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Headers = headers;
        }

        /// <summary>
        /// Gets the backing request header dictionary.
        /// </summary>
        public IHeaderDictionary Headers { get; }

        /// <summary>
        /// Gets or sets the <c>Accept</c> header for an HTTP request.
        /// </summary>
        public IList<MediaTypeHeaderValue> Accept
        {
            get
            {
                return Headers.GetList<MediaTypeHeaderValue>(HeaderNames.Accept);
            }
            set
            {
                Headers.SetList(HeaderNames.Accept, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Accept-Charset</c> header for an HTTP request.
        /// </summary>
        public IList<StringWithQualityHeaderValue> AcceptCharset
        {
            get
            {
                return Headers.GetList<StringWithQualityHeaderValue>(HeaderNames.AcceptCharset);
            }
            set
            {
                Headers.SetList(HeaderNames.AcceptCharset, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Accept-Encoding</c> header for an HTTP request.
        /// </summary>
        public IList<StringWithQualityHeaderValue> AcceptEncoding
        {
            get
            {
                return Headers.GetList<StringWithQualityHeaderValue>(HeaderNames.AcceptEncoding);
            }
            set
            {
                Headers.SetList(HeaderNames.AcceptEncoding, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Accept-Language</c> header for an HTTP request.
        /// </summary>
        public IList<StringWithQualityHeaderValue> AcceptLanguage
        {
            get
            {
                return Headers.GetList<StringWithQualityHeaderValue>(HeaderNames.AcceptLanguage);
            }
            set
            {
                Headers.SetList(HeaderNames.AcceptLanguage, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Cache-Control</c> header for an HTTP request.
        /// </summary>
        public CacheControlHeaderValue? CacheControl
        {
            get
            {
                return Headers.Get<CacheControlHeaderValue>(HeaderNames.CacheControl);
            }
            set
            {
                Headers.Set(HeaderNames.CacheControl, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Content-Disposition</c> header for an HTTP request.
        /// </summary>
        public ContentDispositionHeaderValue? ContentDisposition
        {
            get
            {
                return Headers.Get<ContentDispositionHeaderValue>(HeaderNames.ContentDisposition);
            }
            set
            {
                Headers.Set(HeaderNames.ContentDisposition, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Content-Length</c> header for an HTTP request.
        /// </summary>
        public long? ContentLength
        {
            get
            {
                return Headers.ContentLength;
            }
            set
            {
                Headers.ContentLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the <c>Content-Range</c> header for an HTTP request.
        /// </summary>
        public ContentRangeHeaderValue? ContentRange
        {
            get
            {
                return Headers.Get<ContentRangeHeaderValue>(HeaderNames.ContentRange);
            }
            set
            {
                Headers.Set(HeaderNames.ContentRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Content-Type</c> header for an HTTP request.
        /// </summary>
        public MediaTypeHeaderValue? ContentType
        {
            get
            {
                return Headers.Get<MediaTypeHeaderValue>(HeaderNames.ContentType);
            }
            set
            {
                Headers.Set(HeaderNames.ContentType, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Cookie</c> header for an HTTP request.
        /// </summary>
        public IList<CookieHeaderValue> Cookie
        {
            get
            {
                return Headers.GetList<CookieHeaderValue>(HeaderNames.Cookie);
            }
            set
            {
                Headers.SetList(HeaderNames.Cookie, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Date</c> header for an HTTP request.
        /// </summary>
        public DateTimeOffset? Date
        {
            get
            {
                return Headers.GetDate(HeaderNames.Date);
            }
            set
            {
                Headers.SetDate(HeaderNames.Date, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Expires</c> header for an HTTP request.
        /// </summary>
        public DateTimeOffset? Expires
        {
            get
            {
                return Headers.GetDate(HeaderNames.Expires);
            }
            set
            {
                Headers.SetDate(HeaderNames.Expires, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Host</c> header for an HTTP request.
        /// </summary>
        public HostString Host
        {
            get
            {
                return HostString.FromUriComponent(Headers[HeaderNames.Host]);
            }
            set
            {
                Headers[HeaderNames.Host] = value.ToUriComponent();
            }
        }

        /// <summary>
        /// Gets or sets the <c>If-Match</c> header for an HTTP request.
        /// </summary>
        public IList<EntityTagHeaderValue> IfMatch
        {
            get
            {
                return Headers.GetList<EntityTagHeaderValue>(HeaderNames.IfMatch);
            }
            set
            {
                Headers.SetList(HeaderNames.IfMatch, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>If-Modified-Since</c> header for an HTTP request.
        /// </summary>
        public DateTimeOffset? IfModifiedSince
        {
            get
            {
                return Headers.GetDate(HeaderNames.IfModifiedSince);
            }
            set
            {
                Headers.SetDate(HeaderNames.IfModifiedSince, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>If-None-Match</c> header for an HTTP request.
        /// </summary>
        public IList<EntityTagHeaderValue> IfNoneMatch
        {
            get
            {
                return Headers.GetList<EntityTagHeaderValue>(HeaderNames.IfNoneMatch);
            }
            set
            {
                Headers.SetList(HeaderNames.IfNoneMatch, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>If-Range</c> header for an HTTP request.
        /// </summary>
        public RangeConditionHeaderValue? IfRange
        {
            get
            {
                return Headers.Get<RangeConditionHeaderValue>(HeaderNames.IfRange);
            }
            set
            {
                Headers.Set(HeaderNames.IfRange, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>If-Unmodified-Since</c> header for an HTTP request.
        /// </summary>
        public DateTimeOffset? IfUnmodifiedSince
        {
            get
            {
                return Headers.GetDate(HeaderNames.IfUnmodifiedSince);
            }
            set
            {
                Headers.SetDate(HeaderNames.IfUnmodifiedSince, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Last-Modified</c> header for an HTTP request.
        /// </summary>
        public DateTimeOffset? LastModified
        {
            get
            {
                return Headers.GetDate(HeaderNames.LastModified);
            }
            set
            {
                Headers.SetDate(HeaderNames.LastModified, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Range</c> header for an HTTP request.
        /// </summary>
        public RangeHeaderValue? Range
        {
            get
            {
                return Headers.Get<RangeHeaderValue>(HeaderNames.Range);
            }
            set
            {
                Headers.Set(HeaderNames.Range, value);
            }
        }

        /// <summary>
        /// Gets or sets the <c>Referer</c> header for an HTTP request.
        /// </summary>
        public Uri? Referer
        {
            get
            {
                if (Uri.TryCreate(Headers[HeaderNames.Referer], UriKind.RelativeOrAbsolute, out var uri))
                {
                    return uri;
                }
                return null;
            }
            set
            {
                Headers.Set(HeaderNames.Referer, value == null ? null : UriHelper.Encode(value));
            }
        }

        /// <summary>
        /// Gets the value of header with <paramref name="name"/>.
        /// </summary>
        /// <remarks><typeparamref name="T"/> must contain a TryParse method with the signature <c>public static bool TryParse(string, out T)</c>.</remarks>
        /// <typeparam name="T">The type of the header.
        /// The given type must have a static TryParse method.</typeparam>
        /// <param name="name">The name of the header to retrieve.</param>
        /// <returns>The value of the header.</returns>
        public T? Get<T>(string name)
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
        public IList<T> GetList<T>(string name)
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
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

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
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Headers.SetList<T>(name, values);
        }

        /// <summary>
        /// Appends the header name and value.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public void Append(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

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
}
