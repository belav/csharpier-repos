// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Connections
{
    /// <summary>
    /// <see cref="IConnectionBuilder"/> extensions.
    /// </summary>
    public static class ConnectionBuilderExtensions
    {
        /// <summary>
        /// Use the given <typeparamref name="TConnectionHandler"/> <see cref="ConnectionHandler"/>.
        /// </summary>
        /// <typeparam name="TConnectionHandler">The <see cref="Type"/> of the <see cref="ConnectionHandler"/>.</typeparam>
        /// <param name="connectionBuilder">The <see cref="IConnectionBuilder"/>.</param>
        /// <returns>The <see cref="IConnectionBuilder"/>.</returns>
        public static IConnectionBuilder UseConnectionHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TConnectionHandler>(this IConnectionBuilder connectionBuilder) where TConnectionHandler : ConnectionHandler
        {
            var handler = ActivatorUtilities.GetServiceOrCreateInstance<TConnectionHandler>(connectionBuilder.ApplicationServices);

            // This is a terminal middleware, so there's no need to use the 'next' parameter
            return connectionBuilder.Run(connection => handler.OnConnectedAsync(connection));
        }

        /// <summary>
        /// Add the given <paramref name="middleware"/> to the connection.
        /// </summary>
        /// <param name="connectionBuilder">The <see cref="IConnectionBuilder"/>.</param>
        /// <param name="middleware">The middleware to add to the <see cref="IConnectionBuilder"/>.</param>
        /// <returns>The <see cref="IConnectionBuilder"/>.</returns>
        public static IConnectionBuilder Use(this IConnectionBuilder connectionBuilder, Func<ConnectionContext, Func<Task>, Task> middleware)
        {
            return connectionBuilder.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        /// <summary>
        /// Add the given <paramref name="middleware"/> to the connection.
        /// </summary>
        /// <param name="connectionBuilder">The <see cref="IConnectionBuilder"/>.</param>
        /// <param name="middleware">The middleware to add to the <see cref="IConnectionBuilder"/>.</param>
        /// <returns>The <see cref="IConnectionBuilder"/>.</returns>
        public static IConnectionBuilder Run(this IConnectionBuilder connectionBuilder, Func<ConnectionContext, Task> middleware)
        {
            return connectionBuilder.Use(next =>
            {
                return context =>
                {
                    return middleware(context);
                };
            });
        }
    }
}
