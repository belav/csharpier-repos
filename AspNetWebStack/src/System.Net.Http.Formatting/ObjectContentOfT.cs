// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    /// <summary>
    /// Generic form of <see cref="ObjectContent"/>.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="ObjectContent"/> class will contain.</typeparam>
    public class ObjectContent<T> : ObjectContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="formatter">The formatter to use when serializing the value.</param>
        public ObjectContent(T value, MediaTypeFormatter formatter)
            : this(value, formatter, (MediaTypeHeaderValue)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="formatter">The formatter to use when serializing the value.</param>
        /// <param name="mediaType">The authoritative value of the content's Content-Type header. Can be <c>null</c> in which case the
        /// <paramref name="formatter">formatter's</paramref> default content type will be used.</param>
        public ObjectContent(T value, MediaTypeFormatter formatter, string mediaType)
            : this(value, formatter, BuildHeaderValue(mediaType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContent{T}"/> class.
        /// </summary>
        /// <param name="value">The value of the object this instance will contain.</param>
        /// <param name="formatter">The formatter to use when serializing the value.</param>
        /// <param name="mediaType">The authoritative value of the content's Content-Type header. Can be <c>null</c> in which case the
        /// <paramref name="formatter">formatter's</paramref> default content type will be used.</param>
        public ObjectContent(T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType)
            : base(typeof(T), value, formatter, mediaType)
        {
        }
    }
}