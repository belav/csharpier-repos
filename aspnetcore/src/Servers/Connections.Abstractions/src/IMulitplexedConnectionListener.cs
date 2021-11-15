// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.Connections;

/// <summary>
/// Defines an interface that represents a listener bound to a specific <see cref="EndPoint"/>.
/// </summary>
public interface IMultiplexedConnectionListener : IAsyncDisposable
{
    /// <summary>
    /// The endpoint that was bound. This may differ from the requested endpoint, such as when the caller requested that any free port be selected.
    /// </summary>
    EndPoint EndPoint { get; }

    /// <summary>
    /// Stops listening for incoming connections.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the un-bind operation.</returns>
    ValueTask UnbindAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins an asynchronous operation to accept an incoming connection.
    /// </summary>
    /// <param name="features">A feature collection to pass options when accepting a connection.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask{MultiplexedConnectionContext}"/> that completes when a connection is accepted, yielding the <see cref="MultiplexedConnectionContext" /> representing the connection.</returns>
    ValueTask<MultiplexedConnectionContext?> AcceptAsync(IFeatureCollection? features = null, CancellationToken cancellationToken = default);
}
