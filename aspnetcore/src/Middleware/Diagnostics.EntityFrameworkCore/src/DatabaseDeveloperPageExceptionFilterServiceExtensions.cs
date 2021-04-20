// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service extension methods for the <see cref="DatabaseDeveloperPageExceptionFilter"/>.
    /// </summary>
    public static class DatabaseDeveloperPageExceptionFilterServiceExtensions
    {
        /// <summary>
        /// In combination with UseDeveloperExceptionPage, this captures database-related exceptions that can be resolved by using Entity Framework migrations.
        /// When these exceptions occur, an HTML response with details about possible actions to resolve the issue is generated.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        /// <remarks>
        /// This should only be enabled in the Development environment. 
        /// </remarks>
        public static IServiceCollection AddDatabaseDeveloperPageExceptionFilter(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddEnumerable(new ServiceDescriptor(typeof(IDeveloperPageExceptionFilter), typeof(DatabaseDeveloperPageExceptionFilter), ServiceLifetime.Singleton));

            return services;
        }
    }
}
