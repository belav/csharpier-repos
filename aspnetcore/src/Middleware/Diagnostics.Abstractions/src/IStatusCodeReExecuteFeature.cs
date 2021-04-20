// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Diagnostics
{
    /// <summary>
    /// Represents a feature containing the path details of the original request. This feature is provided by the
    /// StatusCodePagesMiddleware when it re-execute the request pipeline with an alternative path to generate the
    /// response body.
    /// </summary>
    public interface IStatusCodeReExecuteFeature
    {
        /// <summary>
        /// The <see cref="HttpRequest.PathBase"/> of the original request.
        /// </summary>
        string OriginalPathBase { get; set; }

        /// <summary>
        /// The <see cref="HttpRequest.Path"/> of the original request.
        /// </summary>
        string OriginalPath { get; set; }

        /// <summary>
        /// The <see cref="HttpRequest.QueryString"/> of the original request.
        /// </summary>
        string? OriginalQueryString { get; set; }
    }
}
