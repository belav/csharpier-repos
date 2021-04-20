// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.AspNetCore.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Tests
{
    public class Http2StreamStackTests
    {
        [Fact]
        public void RemoveExpired_Empty_NoOp()
        {
            var streams = new Http2StreamStack(10);

            streams.RemoveExpired(100);
        }

        [Fact]
        public void RemoveExpired_NoneExpired_NoOp()
        {
            var streams = new Http2StreamStack(10);
            streams.Push(CreateStream(streamId: 1, expirationTicks: 200));

            streams.RemoveExpired(100);

            Assert.Equal(1, streams.Count);
            Assert.Equal(1, ((Http2Stream)streams._array[0]).StreamId);
        }

        [Fact]
        public void RemoveExpired_OneExpired_ExpiredStreamRemoved()
        {
            var streams = new Http2StreamStack(10);
            streams.Push(CreateStream(streamId: 1, expirationTicks: 200));

            streams.RemoveExpired(300);

            Assert.Equal(0, streams.Count);
            Assert.Equal(default, streams._array[0]);
        }

        [Fact]
        public void RemoveExpired_MultipleExpired_ExpiredStreamsRemoved()
        {
            var streams = new Http2StreamStack(10);
            streams.Push(CreateStream(streamId: 1, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 2, expirationTicks: 250));

            streams.RemoveExpired(300);

            Assert.Equal(0, streams.Count);
            Assert.Equal(default, streams._array[0]);
            Assert.Equal(default, streams._array[1]);
        }

        [Fact]
        public void RemoveExpired_OneExpiredAndOneValid_ExpiredStreamRemoved()
        {
            var streams = new Http2StreamStack(10);
            streams.Push(CreateStream(streamId: 1, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 2, expirationTicks: 400));

            streams.RemoveExpired(300);

            Assert.Equal(1, streams.Count);
            Assert.Equal(2, ((Http2Stream)streams._array[0]).StreamId);
            Assert.Equal(default, streams._array[1]);
        }

        [Fact]
        public void RemoveExpired_AllExpired_ExpiredStreamRemoved()
        {
            var streams = new Http2StreamStack(5);
            streams.Push(CreateStream(streamId: 1, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 2, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 3, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 4, expirationTicks: 200));
            streams.Push(CreateStream(streamId: 5, expirationTicks: 200));

            streams.RemoveExpired(300);

            Assert.Equal(0, streams.Count);
            Assert.Equal(5, streams._array.Length);
            Assert.Equal(default, streams._array[0]);
            Assert.Equal(default, streams._array[1]);
            Assert.Equal(default, streams._array[2]);
            Assert.Equal(default, streams._array[3]);
            Assert.Equal(default, streams._array[4]);
        }

        private static Http2Stream<HttpContext> CreateStream(int streamId, long expirationTicks)
        {
            var context = new Http2StreamContext
            (
                connectionId: "TestConnectionId",
                protocols: HttpProtocols.Http2,
                serviceContext: TestContextFactory.CreateServiceContext(serverOptions: new KestrelServerOptions()),
                connectionFeatures: new FeatureCollection(),
                memoryPool: MemoryPool<byte>.Shared,
                localEndPoint: null,
                remoteEndPoint: null,
                streamId: streamId,
                streamLifetimeHandler: null!,
                clientPeerSettings: new Http2PeerSettings(),
                serverPeerSettings: new Http2PeerSettings(),
                frameWriter: null!,
                connectionInputFlowControl: null!,
                connectionOutputFlowControl: null!
            );

            return new Http2Stream<HttpContext>(new DummyApplication(), context)
            {
                DrainExpirationTicks = expirationTicks
            };
        }
    }
}
