// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.WebUtilities
{
    /// <summary>
    /// A multipart section read by <see cref="MultipartReader"/>.
    /// </summary>
    public class MultipartSection
    {
        /// <summary>
        /// Gets the value of the <c>Content-Type</c> header.
        /// </summary>
        public string? ContentType
        {
            get
            {
                if (Headers != null && Headers.TryGetValue(HeaderNames.ContentType, out var values))
                {
                    return values;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the value of the <c>Content-Disposition</c> header.
        /// </summary>
        public string? ContentDisposition
        {
            get
            {
                if (Headers != null && Headers.TryGetValue(HeaderNames.ContentDisposition, out var values))
                {
                    return values;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the multipart header collection.
        /// </summary>
        public Dictionary<string, StringValues>? Headers { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public Stream Body { get; set; } = default!;

        /// <summary>
        /// The position where the body starts in the total multipart body.
        /// This may not be available if the total multipart body is not seekable.
        /// </summary>
        public long? BaseStreamOffset { get; set; }
    }
}
