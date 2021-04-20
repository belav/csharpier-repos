// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Quic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Experimental.Quic.Internal
{
    internal class QuicStreamContext : TransportConnection, IStreamDirectionFeature, IProtocolErrorCodeFeature, IStreamIdFeature
    {
        private Task _processingTask = Task.CompletedTask;
        private readonly QuicStream _stream;
        private readonly QuicConnectionContext _connection;
        private readonly QuicTransportContext _context;
        private readonly IDuplexPipe _originalTransport;
        private readonly CancellationTokenSource _streamClosedTokenSource = new CancellationTokenSource();
        private readonly IQuicTrace _log;
        private string? _connectionId;
        private const int MinAllocBufferSize = 4096;
        private volatile Exception? _shutdownReason;
        private bool _streamClosed;
        private bool _aborted;
        private readonly TaskCompletionSource _waitForConnectionClosedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly object _shutdownLock = new object();

        public QuicStreamContext(QuicStream stream, QuicConnectionContext connection, QuicTransportContext context)
        {
            _stream = stream;
            _connection = connection;
            _context = context;
            _log = context.Log;
            MemoryPool = connection.MemoryPool;

            ConnectionClosed = _streamClosedTokenSource.Token;

            var maxReadBufferSize = context.Options.MaxReadBufferSize ?? 0;
            var maxWriteBufferSize = context.Options.MaxWriteBufferSize ?? 0;

            // TODO should we allow these PipeScheduler to be configurable here?
            var inputOptions = new PipeOptions(MemoryPool, PipeScheduler.ThreadPool, PipeScheduler.Inline, maxReadBufferSize, maxReadBufferSize / 2, useSynchronizationContext: false);
            var outputOptions = new PipeOptions(MemoryPool, PipeScheduler.Inline, PipeScheduler.ThreadPool, maxWriteBufferSize, maxWriteBufferSize / 2, useSynchronizationContext: false);

            var pair = DuplexPipe.CreateConnectionPair(inputOptions, outputOptions);

            Features.Set<IStreamDirectionFeature>(this);
            Features.Set<IProtocolErrorCodeFeature>(this);
            Features.Set<IStreamIdFeature>(this);

            // TODO populate the ITlsConnectionFeature (requires client certs).
            Features.Set<ITlsConnectionFeature>(new FakeTlsConnectionFeature());
            CanRead = stream.CanRead;
            CanWrite = stream.CanWrite;

            Transport = _originalTransport = pair.Transport;
            Application = pair.Application;
        }

        public override MemoryPool<byte> MemoryPool { get; }
        public PipeWriter Input => Application.Output;
        public PipeReader Output => Application.Input;

        public bool CanRead { get; }
        public bool CanWrite { get; }

        public long StreamId => _stream.StreamId;

        public override string ConnectionId
        {
            get => _connectionId ??= $"{_connection.ConnectionId}:{StreamId}";
            set => _connectionId = value;
        }

        public long Error { get; set; }

        public void Start()
        {
            _processingTask = StartAsync();
        }

        private async Task StartAsync()
        {
            try
            {
                // Spawn send and receive logic
                // Streams may or may not have reading/writing, so only start tasks accordingly
                var receiveTask = Task.CompletedTask;
                var sendTask = Task.CompletedTask;

                if (_stream.CanRead)
                {
                    receiveTask = DoReceive();
                }

                if (_stream.CanWrite)
                {
                    sendTask = DoSend();
                }

                // Now wait for both to complete
                await receiveTask;
                await sendTask;
            }
            catch (Exception ex)
            {
                _log.LogError(0, ex, $"Unexpected exception in {nameof(QuicStreamContext)}.{nameof(StartAsync)}.");
            }
        }

        private async Task DoReceive()
        {
            Exception? error = null;

            try
            {
                await ProcessReceives();
            }
            catch (QuicException ex)
            {
                // This could be ignored if _shutdownReason is already set.
                error = new ConnectionResetException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                // This is unexpected.
                error = ex;
                _log.StreamError(this, error);
            }
            finally
            {
                // If Shutdown() has already bee called, assume that was the reason ProcessReceives() exited.
                Input.Complete(_shutdownReason ?? error);

                FireStreamClosed();

                await _waitForConnectionClosedTcs.Task;
            }
        }

        private async Task ProcessReceives()
        {
            var input = Input;
            while (true)
            {
                var buffer = Input.GetMemory(MinAllocBufferSize);
                var bytesReceived = await _stream.ReadAsync(buffer);

                if (bytesReceived == 0)
                {
                    // Read completed.
                    break;
                }

                input.Advance(bytesReceived);

                var flushTask = input.FlushAsync();

                var paused = !flushTask.IsCompleted;

                if (paused)
                {
                    _log.StreamPause(this);
                }

                var result = await flushTask;

                if (paused)
                {
                    _log.StreamResume(this);
                }

                if (result.IsCompleted || result.IsCanceled)
                {
                    // Pipe consumer is shut down, do we stop writing
                    break;
                }
            }
        }

        private void FireStreamClosed()
        {
            // Guard against scheduling this multiple times
            if (_streamClosed)
            {
                return;
            }

            _streamClosed = true;

            ThreadPool.UnsafeQueueUserWorkItem(state =>
            {
                state.CancelConnectionClosedToken();

                state._waitForConnectionClosedTcs.TrySetResult();
            },
            this,
            preferLocal: false);
        }

        private void CancelConnectionClosedToken()
        {
            try
            {
                _streamClosedTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _log.LogError(0, ex, $"Unexpected exception in {nameof(QuicStreamContext)}.{nameof(CancelConnectionClosedToken)}.");
            }
        }


        private async Task DoSend()
        {
            Exception? shutdownReason = null;
            Exception? unexpectedError = null;

            try
            {
                await ProcessSends();
            }
            catch (QuicException ex)
            {
                shutdownReason = new ConnectionResetException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                shutdownReason = ex;
                unexpectedError = ex;
                _log.ConnectionError(this, unexpectedError);
            }
            finally
            {
                await ShutdownWrite(shutdownReason);

                // Complete the output after disposing the stream
                Output.Complete(unexpectedError);

                // Cancel any pending flushes so that the input loop is un-paused
                Input.CancelPendingFlush();
            }
        }

        private async Task ProcessSends()
        {
            // Resolve `output` PipeReader via the IDuplexPipe interface prior to loop start for performance.
            var output = Output;
            while (true)
            {
                var result = await output.ReadAsync();

                if (result.IsCanceled)
                {
                    break;
                }

                var buffer = result.Buffer;

                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    await _stream.WriteAsync(buffer, endStream: isCompleted);
                }

                output.AdvanceTo(end);

                if (isCompleted)
                {
                    // Once the stream pipe is closed, shutdown the stream.
                    break;
                }
            }
        }

        public override void Abort(ConnectionAbortedException abortReason)
        {
            // This abort is called twice, make sure that doesn't happen.
            // Don't call _stream.Shutdown and _stream.Abort at the same time.
            if (_aborted)
            {
                return;
            }

            _aborted = true;

            _log.StreamAbort(this, abortReason.Message);

            lock (_shutdownLock)
            {
                _stream.AbortRead(Error);
                _stream.AbortWrite(Error);
            }

            // Cancel ProcessSends loop after calling shutdown to ensure the correct _shutdownReason gets set.
            Output.CancelPendingRead();
        }

        private async ValueTask ShutdownWrite(Exception? shutdownReason)
        {
            try
            {
                lock (_shutdownLock)
                {
                    // TODO: Exception is always allocated. Consider only allocating if receive hasn't completed.
                    _shutdownReason = shutdownReason ?? new ConnectionAbortedException("The Quic transport's send loop completed gracefully.");
                    _log.StreamShutdownWrite(this, _shutdownReason.Message);

                    _stream.Shutdown();
                }

                await _stream.ShutdownWriteCompleted();
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Stream failed to gracefully shutdown.");
                // Ignore any errors from Shutdown() since we're tearing down the stream anyway.
            }
        }

        public override async ValueTask DisposeAsync()
        {
            _originalTransport.Input.Complete();
            _originalTransport.Output.Complete();

            await _processingTask;

            _stream.Dispose();

            _streamClosedTokenSource.Dispose();
        }
    }
}
