// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Quic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Experimental.Quic.Internal
{
    internal class QuicConnectionContext : TransportMultiplexedConnection, IProtocolErrorCodeFeature
    {
        private readonly QuicConnection _connection;
        private readonly QuicTransportContext _context;
        private readonly IQuicTrace _log;
        private readonly CancellationTokenSource _connectionClosedTokenSource = new CancellationTokenSource();

        private Task? _closeTask;

        public long Error { get; set; }

        public QuicConnectionContext(QuicConnection connection, QuicTransportContext context)
        {
            _log = context.Log;
            _context = context;
            _connection = connection;
            ConnectionClosed = _connectionClosedTokenSource.Token;
            Features.Set<ITlsConnectionFeature>(new FakeTlsConnectionFeature());
            Features.Set<IProtocolErrorCodeFeature>(this);

            _log.AcceptedConnection(this);
        }

        public ValueTask<ConnectionContext> StartUnidirectionalStreamAsync()
        {
            var stream = _connection.OpenUnidirectionalStream();
            var context = new QuicStreamContext(stream, this, _context);
            context.Start();
            return new ValueTask<ConnectionContext>(context);
        }

        public ValueTask<ConnectionContext> StartBidirectionalStreamAsync()
        {
            var stream = _connection.OpenBidirectionalStream();
            var context = new QuicStreamContext(stream, this, _context);
            context.Start();
            return new ValueTask<ConnectionContext>(context);
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                _closeTask ??= _connection.CloseAsync(errorCode: 0).AsTask();
                await _closeTask;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Failed to gracefully shutdown connection.");
            }

            _connection.Dispose();
        }

        public override void Abort() => Abort(new ConnectionAbortedException("The connection was aborted by the application via MultiplexedConnectionContext.Abort()."));

        public override void Abort(ConnectionAbortedException abortReason)
        {
            // dedup calls to abort here.
            _closeTask = _connection.CloseAsync(errorCode: Error).AsTask();
        }

        public override async ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = await _connection.AcceptStreamAsync(cancellationToken);
                var context = new QuicStreamContext(stream, this, _context);
                context.Start();
                return context;
            }
            catch (QuicConnectionAbortedException ex)
            {
                // Shutdown initiated by peer, abortive.
                // TODO cancel CTS here?
                _log.LogDebug($"Accept loop ended with exception: {ex.Message}");
            }
            catch (QuicOperationAbortedException)
            {
                // Shutdown initiated by us

                // Allow for graceful closure.
            }

            return null;
        }

        public override ValueTask<ConnectionContext> ConnectAsync(IFeatureCollection? features = null, CancellationToken cancellationToken = default)
        {
            QuicStream quicStream;

            if (features != null)
            {
                var streamDirectionFeature = features.Get<IStreamDirectionFeature>()!;
                if (streamDirectionFeature.CanRead)
                {
                    quicStream = _connection.OpenBidirectionalStream();
                }
                else
                {
                    quicStream = _connection.OpenUnidirectionalStream();
                }
            }
            else
            {
                quicStream = _connection.OpenBidirectionalStream();
            }

            var context = new QuicStreamContext(quicStream, this, _context);
            context.Start();

            return new ValueTask<ConnectionContext>(context);
        }
    }
}
