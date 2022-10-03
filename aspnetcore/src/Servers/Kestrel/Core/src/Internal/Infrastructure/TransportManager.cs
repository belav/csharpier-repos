// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Security;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

internal sealed class TransportManager
{
    private readonly List<ActiveTransport> _transports = new List<ActiveTransport>();

    private readonly IConnectionListenerFactory? _transportFactory;
    private readonly IMultiplexedConnectionListenerFactory? _multiplexedTransportFactory;
    private readonly ServiceContext _serviceContext;

    public TransportManager(
        IConnectionListenerFactory? transportFactory,
        IMultiplexedConnectionListenerFactory? multiplexedTransportFactory,
        ServiceContext serviceContext)
    {
        _transportFactory = transportFactory;
        _multiplexedTransportFactory = multiplexedTransportFactory;
        _serviceContext = serviceContext;
    }

    private ConnectionManager ConnectionManager => _serviceContext.ConnectionManager;
    private KestrelTrace Trace => _serviceContext.Log;

    public async Task<EndPoint> BindAsync(EndPoint endPoint, ConnectionDelegate connectionDelegate, EndpointConfig? endpointConfig, CancellationToken cancellationToken)
    {
        if (_transportFactory is null)
        {
            throw new InvalidOperationException($"Cannot bind with {nameof(ConnectionDelegate)} no {nameof(IConnectionListenerFactory)} is registered.");
        }

        var transport = await _transportFactory.BindAsync(endPoint, cancellationToken).ConfigureAwait(false);
        StartAcceptLoop(new GenericConnectionListener(transport), c => connectionDelegate(c), endpointConfig);
        return transport.EndPoint;
    }

    public async Task<EndPoint> BindAsync(EndPoint endPoint, MultiplexedConnectionDelegate multiplexedConnectionDelegate, ListenOptions listenOptions, CancellationToken cancellationToken)
    {
        if (_multiplexedTransportFactory is null)
        {
            throw new InvalidOperationException($"Cannot bind with {nameof(MultiplexedConnectionDelegate)} no {nameof(IMultiplexedConnectionListenerFactory)} is registered.");
        }

        var features = new FeatureCollection();

        // HttpsOptions or HttpsCallbackOptions should always be set in production, but it's not set for InMemory tests.
        // The QUIC transport will check if TlsConnectionCallbackOptions is missing.
        if (listenOptions.HttpsOptions != null)
        {
            var sslServerAuthenticationOptions = HttpsConnectionMiddleware.CreateHttp3Options(listenOptions.HttpsOptions);
            features.Set(new TlsConnectionCallbackOptions
            {
                ApplicationProtocols = sslServerAuthenticationOptions.ApplicationProtocols ?? new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
                OnConnection = (context, cancellationToken) => ValueTask.FromResult(sslServerAuthenticationOptions),
                OnConnectionState = null,
            });
        }
        else if (listenOptions.HttpsCallbackOptions != null)
        {
            features.Set(new TlsConnectionCallbackOptions
            {
                ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
                OnConnection = (context, cancellationToken) =>
                {
                    return listenOptions.HttpsCallbackOptions.OnConnection(new TlsHandshakeCallbackContext
                    {
                        ClientHelloInfo = context.ClientHelloInfo,
                        CancellationToken = cancellationToken,
                        State = context.State,
                        Connection = new ConnectionContextAdapter(context.Connection),
                    });
                },
                OnConnectionState = listenOptions.HttpsCallbackOptions.OnConnectionState,
            });
        }

        var transport = await _multiplexedTransportFactory.BindAsync(endPoint, features, cancellationToken).ConfigureAwait(false);
        StartAcceptLoop(new GenericMultiplexedConnectionListener(transport), c => multiplexedConnectionDelegate(c), listenOptions.EndpointConfig);
        return transport.EndPoint;
    }

    /// <summary>
    /// TlsHandshakeCallbackContext.Connection is ConnectionContext but QUIC connection only implements BaseConnectionContext.
    /// </summary>
    private sealed class ConnectionContextAdapter : ConnectionContext
    {
        private readonly BaseConnectionContext _inner;

        public ConnectionContextAdapter(BaseConnectionContext inner) => _inner = inner;

        public override IDuplexPipe Transport
        {
            get => throw new NotSupportedException("Not supported by HTTP/3 connections.");
            set => throw new NotSupportedException("Not supported by HTTP/3 connections.");
        }
        public override string ConnectionId
        {
            get => _inner.ConnectionId;
            set => _inner.ConnectionId = value;
        }
        public override IFeatureCollection Features => _inner.Features;
        public override IDictionary<object, object?> Items
        {
            get => _inner.Items;
            set => _inner.Items = value;
        }
        public override EndPoint? LocalEndPoint
        {
            get => _inner.LocalEndPoint;
            set => _inner.LocalEndPoint = value;
        }
        public override EndPoint? RemoteEndPoint
        {
            get => _inner.RemoteEndPoint;
            set => _inner.RemoteEndPoint = value;
        }
        public override CancellationToken ConnectionClosed
        {
            get => _inner.ConnectionClosed;
            set => _inner.ConnectionClosed = value;
        }
        public override ValueTask DisposeAsync() => _inner.DisposeAsync();
    }

    private void StartAcceptLoop<T>(IConnectionListener<T> connectionListener, Func<T, Task> connectionDelegate, EndpointConfig? endpointConfig) where T : BaseConnectionContext
    {
        var transportConnectionManager = new TransportConnectionManager(_serviceContext.ConnectionManager);
        var connectionDispatcher = new ConnectionDispatcher<T>(_serviceContext, connectionDelegate, transportConnectionManager);
        var acceptLoopTask = connectionDispatcher.StartAcceptingConnections(connectionListener);

        _transports.Add(new ActiveTransport(connectionListener, acceptLoopTask, transportConnectionManager, endpointConfig));
    }

    public Task StopEndpointsAsync(List<EndpointConfig> endpointsToStop, CancellationToken cancellationToken)
    {
        var transportsToStop = _transports.Where(t => t.EndpointConfig != null && endpointsToStop.Contains(t.EndpointConfig)).ToList();
        return StopTransportsAsync(transportsToStop, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return StopTransportsAsync(new List<ActiveTransport>(_transports), cancellationToken);
    }

    private async Task StopTransportsAsync(List<ActiveTransport> transportsToStop, CancellationToken cancellationToken)
    {
        var tasks = new Task[transportsToStop.Count];

        for (int i = 0; i < transportsToStop.Count; i++)
        {
            tasks[i] = transportsToStop[i].UnbindAsync(cancellationToken);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        async Task StopTransportConnection(ActiveTransport transport)
        {
            if (!await transport.TransportConnectionManager.CloseAllConnectionsAsync(cancellationToken).ConfigureAwait(false))
            {
                Trace.NotAllConnectionsClosedGracefully();

                if (!await transport.TransportConnectionManager.AbortAllConnectionsAsync().ConfigureAwait(false))
                {
                    Trace.NotAllConnectionsAborted();
                }
            }
        }

        for (int i = 0; i < transportsToStop.Count; i++)
        {
            tasks[i] = StopTransportConnection(transportsToStop[i]);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        for (int i = 0; i < transportsToStop.Count; i++)
        {
            tasks[i] = transportsToStop[i].DisposeAsync().AsTask();
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        foreach (var transport in transportsToStop)
        {
            _transports.Remove(transport);
        }
    }

    private sealed class ActiveTransport : IAsyncDisposable
    {
        public ActiveTransport(IConnectionListenerBase transport, Task acceptLoopTask, TransportConnectionManager transportConnectionManager, EndpointConfig? endpointConfig = null)
        {
            ConnectionListener = transport;
            AcceptLoopTask = acceptLoopTask;
            TransportConnectionManager = transportConnectionManager;
            EndpointConfig = endpointConfig;
        }

        public IConnectionListenerBase ConnectionListener { get; }
        public Task AcceptLoopTask { get; }
        public TransportConnectionManager TransportConnectionManager { get; }

        public EndpointConfig? EndpointConfig { get; }

        public async Task UnbindAsync(CancellationToken cancellationToken)
        {
            await ConnectionListener.UnbindAsync(cancellationToken).ConfigureAwait(false);
            await AcceptLoopTask.ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            return ConnectionListener.DisposeAsync();
        }
    }

    private sealed class GenericConnectionListener : IConnectionListener<ConnectionContext>
    {
        private readonly IConnectionListener _connectionListener;

        public GenericConnectionListener(IConnectionListener connectionListener)
        {
            _connectionListener = connectionListener;
        }

        public EndPoint EndPoint => _connectionListener.EndPoint;

        public ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
             => _connectionListener.AcceptAsync(cancellationToken);

        public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
            => _connectionListener.UnbindAsync(cancellationToken);

        public ValueTask DisposeAsync()
            => _connectionListener.DisposeAsync();
    }

    private sealed class GenericMultiplexedConnectionListener : IConnectionListener<MultiplexedConnectionContext>
    {
        private readonly IMultiplexedConnectionListener _multiplexedConnectionListener;

        public GenericMultiplexedConnectionListener(IMultiplexedConnectionListener multiplexedConnectionListener)
        {
            _multiplexedConnectionListener = multiplexedConnectionListener;
        }

        public EndPoint EndPoint => _multiplexedConnectionListener.EndPoint;

        public ValueTask<MultiplexedConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default)
             => _multiplexedConnectionListener.AcceptAsync(features: null, cancellationToken);

        public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
            => _multiplexedConnectionListener.UnbindAsync(cancellationToken);

        public ValueTask DisposeAsync()
            => _multiplexedConnectionListener.DisposeAsync();
    }
}
