// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
{
    /// <summary>
    /// An <see cref="IDeveloperPageExceptionFilter"/> that captures database-related exceptions that can be resolved by using Entity Framework migrations.
    /// When these exceptions occur, an HTML response with details about possible actions to resolve the issue is generated.
    /// </summary>
    /// <remarks>
    /// This should only be enabled in the Development environment. 
    /// </remarks>
    public sealed class DatabaseDeveloperPageExceptionFilter : IDeveloperPageExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly DatabaseErrorPageOptions _options;

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseDeveloperPageExceptionFilter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="options">The <see cref="IOptions{DatabaseErrorPageOptions}"/>.</param>
        public DatabaseDeveloperPageExceptionFilter(ILogger<DatabaseDeveloperPageExceptionFilter> logger, IOptions<DatabaseErrorPageOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Handle <see cref="DbException"/> errors and output an HTML response with additional details.
        /// </summary>
        /// <inheritdoc />
        public async Task HandleExceptionAsync(ErrorContext errorContext, Func<ErrorContext, Task> next)
        {
            if (!(errorContext.Exception is DbException))
            {
                await next(errorContext);
            }

            try
            {
                // Look for DbContext classes registered in the service provider
                var registeredContexts = errorContext.HttpContext.RequestServices.GetServices<DbContextOptions>()
                    .Select(o => o.ContextType)
                    .Distinct(); // Workaround for https://github.com/dotnet/efcore/issues/22341

                if (registeredContexts.Any())
                {
                    var contextDetails = new List<DatabaseContextDetails>();

                    foreach (var registeredContext in registeredContexts)
                    {
                        var details = await errorContext.HttpContext.GetContextDetailsAsync(registeredContext, _logger);

                        if (details != null)
                        {
                            contextDetails.Add(details);
                        }
                    }

                    if (contextDetails.Any(c => c.PendingModelChanges || c.PendingMigrations.Any()))
                    {
                        var page = new DatabaseErrorPage
                        {
                            Model = new DatabaseErrorPageModel(errorContext.Exception, contextDetails, _options, errorContext.HttpContext.Request.PathBase)
                        };

                        await page.ExecuteAsync(errorContext.HttpContext);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.DatabaseErrorPageMiddlewareException(e);
                return;
            }
        }
    }
}
