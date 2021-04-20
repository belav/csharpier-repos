// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.AspNetCore.Testing;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class Http2KeepAliveTests : Http2TestBase
    {
        [Fact]
        public async Task KeepAlivePingDelay_InfiniteTimeSpan_KeepAliveNotEnabled()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = Timeout.InfiniteTimeSpan;

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            Assert.Null(_connection._keepAlive);

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task KeepAlivePingTimeout_InfiniteTimeSpan_NoGoAway()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingTimeout = Timeout.InfiniteTimeSpan;

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            // Heartbeat that exceeds timeout
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 4));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 20));

            Assert.Equal(KeepAliveState.PingSent, _connection._keepAlive._state);

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_WithoutActivity_PingSent()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_WithActivity_NoPingSent()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            await SendPingAsync(Http2PingFrameFlags.NONE).DefaultTimeout();
            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.ACK,
                withStreamId: 0).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1));

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalNotExceeded_NoPingSent()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(5);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = new DateTimeOffset(1, TimeSpan.Zero);

            // Heartbeats
            TriggerTick(now);
            TriggerTick(now + TimeSpan.FromSeconds(1.1));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_MultipleTimes_PingsNotSentWhileAwaitingOnAck()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            TriggerTick(now);
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 4));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 5));

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_MultipleTimes_PingSentAfterAck()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeats
            TriggerTick(now);
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();
            await SendPingAsync(Http2PingFrameFlags.ACK).DefaultTimeout();

            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 4));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();
            await SendPingAsync(Http2PingFrameFlags.ACK).DefaultTimeout();

            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 5));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 6));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            await StopConnectionAsync(expectedLastStreamId: 0, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task TimeoutExceeded_NoAck_GoAway()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(3);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            // Heartbeat that exceeds timeout
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 4));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 5));
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 6));

            Assert.Equal(KeepAliveState.Timeout, _connection._keepAlive._state);

            VerifyGoAway(await ReceiveFrameAsync().DefaultTimeout(), 0, Http2ErrorCode.INTERNAL_ERROR);
        }

        [Fact]
        public async Task TimeoutExceeded_NonPingActivity_NoGoAway()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(3);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            Assert.Equal(KeepAliveState.PingSent, _connection._keepAlive._state);
            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            await StartStreamAsync(1, _browserRequestHeaders, endStream: true).DefaultTimeout();
            Assert.Equal(KeepAliveState.None, _connection._keepAlive._state);

            await ExpectAsync(Http2FrameType.HEADERS,
                withLength: 36,
                withFlags: (byte)(Http2HeadersFrameFlags.END_HEADERS | Http2HeadersFrameFlags.END_STREAM),
                withStreamId: 1).DefaultTimeout();

            await StopConnectionAsync(expectedLastStreamId: 1, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_StreamStarted_NoPingSent()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            await InitializeConnectionAsync(_noopApplication).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            await StartStreamAsync(1, _browserRequestHeaders, endStream: true).DefaultTimeout();

            await ExpectAsync(Http2FrameType.HEADERS,
                withLength: 36,
                withFlags: (byte)(Http2HeadersFrameFlags.END_HEADERS | Http2HeadersFrameFlags.END_STREAM),
                withStreamId: 1).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1));

            await StopConnectionAsync(expectedLastStreamId: 1, ignoreNonGoAwayFrames: false).DefaultTimeout();
        }

        [Fact]
        public async Task IntervalExceeded_ConnectionFlowControlUsedUpThenPings_NoPingSent()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            // Reduce connection window size so that one stream can fill it
            _serviceContext.ServerOptions.Limits.Http2.InitialConnectionWindowSize = 65535;

            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            await InitializeConnectionAsync(async c =>
            {
                // Don't consume any request data
                await tcs.Task;
                // Send headers
                await c.Request.Body.FlushAsync();
            }, expectedWindowUpdate: false).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            await StartStreamAsync(1, _browserRequestHeaders, endStream: false).DefaultTimeout();

            // Use up connection flow control
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16383], false).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1));

            // Send ping that will update the keep alive on the server
            await SendPingAsync(Http2PingFrameFlags.NONE).DefaultTimeout();
            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.ACK,
                withStreamId: 0).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            // Continue request delegate on server
            tcs.SetResult(null);

            await ExpectAsync(Http2FrameType.HEADERS,
                withLength: 36,
                withFlags: (byte)(Http2HeadersFrameFlags.END_HEADERS | Http2HeadersFrameFlags.END_STREAM),
                withStreamId: 1).DefaultTimeout();

            // Server could send RST_STREAM
            await StopConnectionAsync(expectedLastStreamId: 1, ignoreNonGoAwayFrames: true).DefaultTimeout();
        }

        [Fact]
        public async Task TimeoutExceeded_ConnectionFlowControlUsedUpThenPings_NoGoAway()
        {
            _serviceContext.ServerOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(1);

            // Reduce connection window size so that one stream can fill it
            _serviceContext.ServerOptions.Limits.Http2.InitialConnectionWindowSize = 65535;

            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            await InitializeConnectionAsync(async c =>
            {
                // Don't consume any request data
                await tcs.Task;
                // Send headers
                await c.Request.Body.FlushAsync();
            }, expectedWindowUpdate: false).DefaultTimeout();

            DateTimeOffset now = _serviceContext.MockSystemClock.UtcNow;

            // Heartbeat
            TriggerTick(now);

            await StartStreamAsync(1, _browserRequestHeaders, endStream: false).DefaultTimeout();

            // Use up connection flow control
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16384], false).DefaultTimeout();
            await SendDataAsync(1, new byte[16383], false).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1));

            // Heartbeat that triggers keep alive ping
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 2));

            await ExpectAsync(Http2FrameType.PING,
                withLength: 8,
                withFlags: (byte)Http2PingFrameFlags.NONE,
                withStreamId: 0).DefaultTimeout();

            // Send ping ack that will reset the keep alive on the server
            await SendPingAsync(Http2PingFrameFlags.ACK).DefaultTimeout();

            // Heartbeat that exceeds interval
            TriggerTick(now + TimeSpan.FromSeconds(1.1 * 3));

            // Continue request delegate on server
            tcs.SetResult(null);

            await ExpectAsync(Http2FrameType.HEADERS,
                withLength: 36,
                withFlags: (byte)(Http2HeadersFrameFlags.END_HEADERS | Http2HeadersFrameFlags.END_STREAM),
                withStreamId: 1).DefaultTimeout();

            // Server could send RST_STREAM
            await StopConnectionAsync(expectedLastStreamId: 1, ignoreNonGoAwayFrames: true).DefaultTimeout();
        }
    }
}
