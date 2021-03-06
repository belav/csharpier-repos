// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http.Formatting.Parsers;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    /// <summary>
    /// Represents the HTTP Status Line and header parameters parsed by <see cref="HttpStatusLineParser"/>
    /// and <see cref="HttpResponseHeaderParser"/>.
    /// </summary>
    internal class HttpUnsortedResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpUnsortedRequest"/> class.
        /// </summary>
        public HttpUnsortedResponse()
        {
            // Collection of unsorted headers. Later we will sort it into the appropriate
            // HttpContentHeaders, HttpRequestHeaders, and HttpResponseHeaders.
            HttpHeaders = new HttpUnsortedHeaders();
        }

        /// <summary>
        /// Gets or sets the HTTP version.
        /// </summary>
        /// <value>
        /// The HTTP version.
        /// </value>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HttpStatusCode"/>
        /// </summary>
        /// <value>
        /// The HTTP status code
        /// </value>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP reason phrase
        /// </summary>
        /// <value>
        /// The response reason phrase
        /// </value>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets the unsorted HTTP request headers.
        /// </summary>
        public HttpHeaders HttpHeaders { get; private set; }
    }
}
