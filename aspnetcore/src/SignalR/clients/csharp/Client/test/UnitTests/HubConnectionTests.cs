// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.AspNetCore.SignalR.Tests;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.SignalR.Client.Tests
{
    public partial class HubConnectionTests : VerifiableLoggedTest
    {
        [Fact]
        public async Task InvokeThrowsIfSerializingMessageFails()
        {
            bool ExpectedErrors(WriteContext writeContext)
            {
                return writeContext.LoggerName == typeof(HubConnection).FullName &&
                       writeContext.EventId.Name == "FailedToSendInvocation";
            }
            using (StartVerifiableLog(ExpectedErrors))
            {
                var exception = new InvalidOperationException();
                var hubConnection = CreateHubConnection(new TestConnection(), protocol: MockHubProtocol.Throw(exception), LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var actualException =
                    await Assert.ThrowsAsync<InvalidOperationException>(async () => await hubConnection.InvokeAsync<int>("test").DefaultTimeout());
                Assert.Same(exception, actualException);
            }
        }

        [Fact]
        public async Task SendAsyncThrowsIfSerializingMessageFails()
        {
            using (StartVerifiableLog())
            {
                var exception = new InvalidOperationException();
                var hubConnection = CreateHubConnection(new TestConnection(), protocol: MockHubProtocol.Throw(exception), LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var actualException =
                    await Assert.ThrowsAsync<InvalidOperationException>(async () => await hubConnection.SendAsync("test").DefaultTimeout());
                Assert.Same(exception, actualException);
            }
        }

        [Fact]
        public async Task ClosedEventRaisedWhenTheClientIsStopped()
        {
            var builder = new HubConnectionBuilder().WithUrl("http://example.com");

            var delegateConnectionFactory = new DelegateConnectionFactory(
                endPoint => new TestConnection().StartAsync());
            builder.Services.AddSingleton<IConnectionFactory>(delegateConnectionFactory);

            var hubConnection = builder.Build();
            var closedEventTcs = new TaskCompletionSource<Exception>();
            hubConnection.Closed += e =>
            {
                closedEventTcs.SetResult(e);
                return Task.CompletedTask;
            };

            await hubConnection.StartAsync().DefaultTimeout();
            await hubConnection.StopAsync().DefaultTimeout();
            Assert.Null(await closedEventTcs.Task);
        }

        [Fact]
        public async Task StopAsyncCanBeCalledFromOnHandler()
        {
            var connection = new TestConnection();
            var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            hubConnection.On("method", async () =>
            {
                await hubConnection.StopAsync().DefaultTimeout();
                tcs.SetResult();
            });

            await hubConnection.StartAsync().DefaultTimeout();

            await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.InvocationMessageType, target= "method", arguments = new object[] { } }).DefaultTimeout();

            await tcs.Task.DefaultTimeout();
        }

        [Fact]
        public async Task StopAsyncDoesNotWaitForOnHandlers()
        {
            var connection = new TestConnection();
            var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var methodCalledTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            hubConnection.On("method", async () =>
            {
                methodCalledTcs.SetResult();
                await tcs.Task;
            });

            await hubConnection.StartAsync().DefaultTimeout();

            await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.InvocationMessageType, target = "method", arguments = new object[] { } }).DefaultTimeout();

            await methodCalledTcs.Task.DefaultTimeout();
            await hubConnection.StopAsync().DefaultTimeout();

            tcs.SetResult();
        }

        [Fact]
        public async Task PendingInvocationsAreCanceledWhenConnectionClosesCleanly()
        {
            using (StartVerifiableLog())
            {
                var hubConnection = CreateHubConnection(new TestConnection(), loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                var invokeTask = hubConnection.InvokeAsync<int>("testMethod").DefaultTimeout();
                await hubConnection.StopAsync().DefaultTimeout();

                await Assert.ThrowsAsync<TaskCanceledException>(async () => await invokeTask);
            }
        }

        [Fact]
        public async Task PendingInvocationsAreTerminatedWithExceptionWhenTransportCompletesWithError()
        {
            bool ExpectedErrors(WriteContext writeContext)
            {
                return writeContext.LoggerName == typeof(HubConnection).FullName &&
                       (writeContext.EventId.Name == "ShutdownWithError" ||
                       writeContext.EventId.Name == "ServerDisconnectedWithError");
            }
            using (StartVerifiableLog(ExpectedErrors))
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, protocol: Mock.Of<IHubProtocol>(), LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                var invokeTask = hubConnection.InvokeAsync<int>("testMethod").DefaultTimeout();

                var exception = new InvalidOperationException();
                connection.CompleteFromTransport(exception);

                var actualException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await invokeTask);
                Assert.Equal(exception, actualException);
            }
        }

        [Fact]
        public async Task PendingInvocationsAreCanceledWhenTokenTriggered()
        {
            using (StartVerifiableLog())
            {
                var hubConnection = CreateHubConnection(new TestConnection(), loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                var cts = new CancellationTokenSource();
                var invokeTask = hubConnection.InvokeAsync<int>("testMethod", cancellationToken: cts.Token).DefaultTimeout();
                cts.Cancel();

                await Assert.ThrowsAsync<TaskCanceledException>(async () => await invokeTask);
            }
        }

        [Fact]
        public async Task InvokeAsyncCanceledWhenPassedCanceledToken()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    hubConnection.InvokeAsync<int>("testMethod", cancellationToken: new CancellationToken(canceled: true)).DefaultTimeout());

                await hubConnection.StopAsync().DefaultTimeout();

                // Assert that InvokeAsync didn't send a message
                Assert.Null(await connection.ReadSentTextMessageAsync().DefaultTimeout());
            }
        }

        [Fact]
        public async Task SendAsyncCanceledWhenPassedCanceledToken()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    hubConnection.SendAsync("testMethod", cancellationToken: new CancellationToken(canceled: true)).DefaultTimeout());

                await hubConnection.StopAsync().DefaultTimeout();

                // Assert that SendAsync didn't send a message
                Assert.Null(await connection.ReadSentTextMessageAsync().DefaultTimeout());
            }
        }

        [Fact]
        public async Task StreamAsChannelAsyncCanceledWhenPassedCanceledToken()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                await Assert.ThrowsAsync<TaskCanceledException>(() =>
                    hubConnection.StreamAsChannelAsync<int>("testMethod", cancellationToken: new CancellationToken(canceled: true)).DefaultTimeout());

                await hubConnection.StopAsync().DefaultTimeout();

                // Assert that StreamAsChannelAsync didn't send a message
                Assert.Null(await connection.ReadSentTextMessageAsync().DefaultTimeout());
            }
        }

        [Fact]
        public async Task StreamAsyncCanceledWhenPassedCanceledToken()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();
                var result = hubConnection.StreamAsync<int>("testMethod", cancellationToken: new CancellationToken(canceled: true));
                await Assert.ThrowsAsync<TaskCanceledException>(() => result.GetAsyncEnumerator().MoveNextAsync().DefaultTimeout());

                await hubConnection.StopAsync().DefaultTimeout();

                // Assert that StreamAsync didn't send a message
                Assert.Null(await connection.ReadSentTextMessageAsync().DefaultTimeout());
            }
        }

        [Fact]
        public async Task CanCancelTokenAfterStreamIsCompleted()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);

                await hubConnection.StartAsync().DefaultTimeout();

                var asyncEnumerable = hubConnection.StreamAsync<int>("Stream", 1);
                using var cts = new CancellationTokenSource();
                await using var e = asyncEnumerable.GetAsyncEnumerator(cts.Token);
                var task = e.MoveNextAsync();

                var item = await connection.ReadSentJsonAsync().DefaultTimeout();
                await connection.ReceiveJsonMessage(
                    new { type = HubProtocolConstants.CompletionMessageType, invocationId = item["invocationId"] }
                    ).DefaultTimeout();

                await task.DefaultTimeout();

                while (await e.MoveNextAsync().DefaultTimeout())
                {
                }
                // Cancel after stream is completed but before the AsyncEnumerator is disposed
                cts.Cancel();
            }
        }

        [Fact]
        public async Task ConnectionTerminatedIfServerTimeoutIntervalElapsesWithNoMessages()
        {
            bool ExpectedErrors(WriteContext writeContext)
            {
                return writeContext.LoggerName == typeof(HubConnection).FullName &&
                       writeContext.EventId.Name == "ShutdownWithError";
            }
            using (StartVerifiableLog(ExpectedErrors))
            {
                var hubConnection = CreateHubConnection(new TestConnection(), loggerFactory: LoggerFactory);
                hubConnection.ServerTimeout = TimeSpan.FromMilliseconds(100);

                var closeTcs = new TaskCompletionSource<Exception>();
                hubConnection.Closed += ex =>
                {
                    closeTcs.TrySetResult(ex);
                    return Task.CompletedTask;
                };

                await hubConnection.StartAsync().DefaultTimeout();

                var exception = Assert.IsType<TimeoutException>(await closeTcs.Task.DefaultTimeout());

                // We use an interpolated string so the tests are accurate on non-US machines.
                Assert.Equal($"Server timeout ({hubConnection.ServerTimeout.TotalMilliseconds:0.00}ms) elapsed without receiving a message from the server.", exception.Message);
            }
        }

        [Fact]
        public async Task ServerTimeoutIsDisabledWhenUsingTransportWithInherentKeepAlive()
        {
            using (StartVerifiableLog())
            {
                var testConnection = new TestConnection();
                testConnection.Features.Set<IConnectionInherentKeepAliveFeature>(new TestKeepAliveFeature() { HasInherentKeepAlive = true });
                var hubConnection = CreateHubConnection(testConnection, loggerFactory: LoggerFactory);
                hubConnection.ServerTimeout = TimeSpan.FromMilliseconds(1);

                await hubConnection.StartAsync().DefaultTimeout();

                var closeTcs = new TaskCompletionSource<Exception>();
                hubConnection.Closed += ex =>
                {
                    closeTcs.TrySetResult(ex);
                    return Task.CompletedTask;
                };

                await hubConnection.RunTimerActions().DefaultTimeout();

                Assert.False(closeTcs.Task.IsCompleted);

                await hubConnection.DisposeAsync().DefaultTimeout();
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task PendingInvocationsAreTerminatedIfServerTimeoutIntervalElapsesWithNoMessages()
        {
            bool ExpectedErrors(WriteContext writeContext)
            {
                return writeContext.LoggerName == typeof(HubConnection).FullName &&
                       writeContext.EventId.Name == "ShutdownWithError";
            }

            using (StartVerifiableLog(expectedErrorsFilter: ExpectedErrors))
            {
                var hubConnection = CreateHubConnection(new TestConnection(), loggerFactory: LoggerFactory);
                hubConnection.ServerTimeout = TimeSpan.FromMilliseconds(2000);

                await hubConnection.StartAsync().DefaultTimeout();

                // Start an invocation (but we won't complete it)
                var invokeTask = hubConnection.InvokeAsync("Method").DefaultTimeout();

                var exception = await Assert.ThrowsAsync<TimeoutException>(() => invokeTask);

                // We use an interpolated string so the tests are accurate on non-US machines.
                Assert.Equal($"Server timeout ({hubConnection.ServerTimeout.TotalMilliseconds:0.00}ms) elapsed without receiving a message from the server.", exception.Message);
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task StreamIntsToServer()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var channel = Channel.CreateUnbounded<int>();
                var invokeTask = hubConnection.InvokeAsync<int>("SomeMethod", channel.Reader);

                var invocation = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invocation["type"]);
                Assert.Equal("SomeMethod", invocation["target"]);
                var streamId = invocation["streamIds"][0];

                foreach (var number in new[] { 42, 43, 322, 3145, -1234 })
                {
                    await channel.Writer.WriteAsync(number).AsTask().DefaultTimeout();

                    var item = await connection.ReadSentJsonAsync().DefaultTimeout();
                    Assert.Equal(HubProtocolConstants.StreamItemMessageType, item["type"]);
                    Assert.Equal(number, item["item"]);
                    Assert.Equal(streamId, item["invocationId"]);
                }

                channel.Writer.TryComplete();
                var completion = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.CompletionMessageType, completion["type"]);

                await connection.ReceiveJsonMessage(
                    new { type = HubProtocolConstants.CompletionMessageType, invocationId = invocation["invocationId"], result = 42 }
                    ).DefaultTimeout();
                var result = await invokeTask.DefaultTimeout();
                Assert.Equal(42, result);
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task StreamIntsToServerViaSend()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var channel = Channel.CreateUnbounded<int>();
                var sendTask = hubConnection.SendAsync("SomeMethod", channel.Reader);

                var invocation = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invocation["type"]);
                Assert.Equal("SomeMethod", invocation["target"]);
                Assert.Null(invocation["invocationId"]);
                var streamId = invocation["streamIds"][0];

                foreach (var item in new[] { 2, 3, 10, 5 })
                {
                    await channel.Writer.WriteAsync(item);

                    var received = await connection.ReadSentJsonAsync().DefaultTimeout();
                    Assert.Equal(HubProtocolConstants.StreamItemMessageType, received["type"]);
                    Assert.Equal(item, received["item"]);
                    Assert.Equal(streamId, received["invocationId"]);
                }
            }
        }

        [Fact(Skip = "Objects not supported yet")]
        [LogLevel(LogLevel.Trace)]
        public async Task StreamsObjectsToServer()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var channel = Channel.CreateUnbounded<object>();
                var invokeTask = hubConnection.InvokeAsync<SampleObject>("UploadMethod", channel.Reader);

                var invocation = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invocation["type"]);
                Assert.Equal("UploadMethod", invocation["target"]);
                var id = invocation["invocationId"];

                var items = new[] { new SampleObject("ab", 12), new SampleObject("ef", 23) };
                foreach (var item in items)
                {
                    await channel.Writer.WriteAsync(item);

                    var received = await connection.ReadSentJsonAsync().DefaultTimeout();
                    Assert.Equal(HubProtocolConstants.StreamItemMessageType, received["type"]);
                    Assert.Equal(item.Foo, received["item"]["foo"]);
                    Assert.Equal(item.Bar, received["item"]["bar"]);
                }

                channel.Writer.TryComplete();
                var completion = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.CompletionMessageType, completion["type"]);

                var expected = new SampleObject("oof", 14);
                await connection.ReceiveJsonMessage(
                    new { type = HubProtocolConstants.CompletionMessageType, invocationId = id, result = expected }
                    ).DefaultTimeout();
                var result = await invokeTask.DefaultTimeout();

                Assert.Equal(expected.Foo, result.Foo);
                Assert.Equal(expected.Bar, result.Bar);
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task UploadStreamCancellationSendsStreamComplete()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var cts = new CancellationTokenSource();
                var channel = Channel.CreateUnbounded<int>();
                var invokeTask = hubConnection.InvokeAsync<object>("UploadMethod", channel.Reader, cts.Token);

                var invokeMessage = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invokeMessage["type"]);

                cts.Cancel();

                // after cancellation, don't send from the pipe
                foreach (var number in new[] { 42, 43, 322, 3145, -1234 })
                {
                    await channel.Writer.WriteAsync(number);
                }

                // the next sent message should be a completion message
                var complete = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.CompletionMessageType, complete["type"]);
                Assert.EndsWith("canceled by client.", ((string)complete["error"]));
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task InvocationCanCompleteBeforeStreamCompletes()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var channel = Channel.CreateUnbounded<int>();
                var invokeTask = hubConnection.InvokeAsync<long>("UploadMethod", channel.Reader);
                var invocation = await connection.ReadSentJsonAsync().DefaultTimeout();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invocation["type"]);
                var id = invocation["invocationId"];

                await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.CompletionMessageType, invocationId = id, result = 10 });

                var result = await invokeTask.DefaultTimeout();
                Assert.Equal(10L, result);

                // after the server returns, with whatever response
                // the client's behavior is undefined, and the server is responsible for ignoring stray messages
            }
        }

        [Fact]
        [LogLevel(LogLevel.Trace)]
        public async Task WrongTypeOnServerResponse()
        {
            bool ExpectedErrors(WriteContext writeContext)
            {
                return writeContext.LoggerName == typeof(HubConnection).FullName &&
                       (writeContext.EventId.Name == "ServerDisconnectedWithError"
                        || writeContext.EventId.Name == "ShutdownWithError");
            }
            using (StartVerifiableLog(ExpectedErrors))
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                // we expect to get sent ints, and receive an int back
                var channel = Channel.CreateUnbounded<int>();
                var invokeTask = hubConnection.InvokeAsync<int>("SumInts", channel.Reader);

                var invocation = await connection.ReadSentJsonAsync();
                Assert.Equal(HubProtocolConstants.InvocationMessageType, invocation["type"]);
                var id = invocation["invocationId"];

                await channel.Writer.WriteAsync(5);
                await channel.Writer.WriteAsync(10);

                await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.CompletionMessageType, invocationId = id, result = "humbug" });

                try
                {
                    await invokeTask;
                    Assert.True(false);
                }
                catch (Exception)
                {
                }
            }
        }

        [Fact]
        public async Task CanAwaitInvokeFromOnHandlerWithServerClosingConnection()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();

                var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                hubConnection.On<string>("Echo", async msg =>
                {
                    try
                    {
                        // This should be canceled when the connection is closed
                        await hubConnection.InvokeAsync<string>("Echo", msg).DefaultTimeout();
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                        return;
                    }

                    tcs.SetResult();
                });

                var closedTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                hubConnection.Closed += _ =>
                {
                    closedTcs.SetResult();

                    return Task.CompletedTask;
                };

                await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.InvocationMessageType, target = "Echo", arguments = new object[] { "42" } }).DefaultTimeout();

                // Read sent message first to make sure invoke has been processed and is waiting for a response
                await connection.ReadSentJsonAsync().DefaultTimeout();
                await connection.ReceiveJsonMessage(new { type = HubProtocolConstants.CloseMessageType }).DefaultTimeout();

                await closedTcs.Task.DefaultTimeout();

                await Assert.ThrowsAsync<TaskCanceledException>(() => tcs.Task.DefaultTimeout());
            }
        }

        [Fact]
        public async Task CanAwaitUsingHubConnection()
        {
            using (StartVerifiableLog())
            {
                var connection = new TestConnection();
                await using var hubConnection = CreateHubConnection(connection, loggerFactory: LoggerFactory);
                await hubConnection.StartAsync().DefaultTimeout();
            }
        }

        private class SampleObject
        {
            public SampleObject(string foo, int bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public string Foo { get; private set; }
            public int Bar { get; private set; }
        }

        private struct TestKeepAliveFeature : IConnectionInherentKeepAliveFeature
        {
            public bool HasInherentKeepAlive { get; set; }
        }

        // Moq really doesn't handle out parameters well, so to make these tests work I added a manual mock -anurse
        private class MockHubProtocol : IHubProtocol
        {
            private HubInvocationMessage _parsed;
            private Exception _error;

            public static MockHubProtocol ReturnOnParse(HubInvocationMessage parsed)
            {
                return new MockHubProtocol
                {
                    _parsed = parsed
                };
            }

            public static MockHubProtocol Throw(Exception error)
            {
                return new MockHubProtocol
                {
                    _error = error
                };
            }

            public string Name => "MockHubProtocol";
            public int Version => 1;
            public int MinorVersion => 1;

            public TransferFormat TransferFormat => TransferFormat.Binary;

            public bool IsVersionSupported(int version)
            {
                return true;
            }

            public bool TryParseMessage(ref ReadOnlySequence<byte> input, IInvocationBinder binder, out HubMessage message)
            {
                if (_error != null)
                {
                    throw _error;
                }
                if (_parsed != null)
                {
                    message = _parsed;
                    return true;
                }

                throw new InvalidOperationException("No Parsed Message provided");
            }

            public void WriteMessage(HubMessage message, IBufferWriter<byte> output)
            {
                if (_error != null)
                {
                    throw _error;
                }
            }

            public ReadOnlyMemory<byte> GetMessageBytes(HubMessage message)
            {
                return HubProtocolExtensions.GetMessageBytes(this, message);
            }
        }
    }
}
