// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// A factory for creating <see cref="HttpContext" /> instances.
    /// </summary>
    public class DefaultHttpContextFactory : IHttpContextFactory
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly FormOptions _formOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // This takes the IServiceProvider because it needs to support an ever expanding
        // set of services that flow down into HttpContext features
        /// <summary>
        /// Creates a factory for creating <see cref="HttpContext" /> instances.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to be used when retrieving services.</param>
        public DefaultHttpContextFactory(IServiceProvider serviceProvider)
        {
            // May be null
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            _formOptions = serviceProvider.GetRequiredService<IOptions<FormOptions>>().Value;
            _serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        internal IHttpContextAccessor? HttpContextAccessor => _httpContextAccessor;

        /// <summary>
        /// Create an <see cref="HttpContext"/> instance given an <paramref name="featureCollection" />.
        /// </summary>
        /// <param name="featureCollection"></param>
        /// <returns>An initialized <see cref="HttpContext"/> object.</returns>
        public HttpContext Create(IFeatureCollection featureCollection)
        {
            if (featureCollection is null)
            {
                throw new ArgumentNullException(nameof(featureCollection));
            }

            var httpContext = new DefaultHttpContext(featureCollection);
            Initialize(httpContext);
            return httpContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Initialize(DefaultHttpContext httpContext, IFeatureCollection featureCollection)
        {
            Debug.Assert(featureCollection != null);
            Debug.Assert(httpContext != null);

            httpContext.Initialize(featureCollection);

            Initialize(httpContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DefaultHttpContext Initialize(DefaultHttpContext httpContext)
        {
            if (_httpContextAccessor != null)
            {
                _httpContextAccessor.HttpContext = httpContext;
            }

            httpContext.FormOptions = _formOptions;
            httpContext.ServiceScopeFactory = _serviceScopeFactory;

            return httpContext;
        }

        /// <summary>
        /// Clears the current <see cref="HttpContext" />.
        /// </summary>
        public void Dispose(HttpContext httpContext)
        {
            if (_httpContextAccessor != null)
            {
                _httpContextAccessor.HttpContext = null;
            }
        }

        internal void Dispose(DefaultHttpContext httpContext)
        {
            if (_httpContextAccessor != null)
            {
                _httpContextAccessor.HttpContext = null;
            }

            httpContext.Uninitialize();
        }
    }
}
