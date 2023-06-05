// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.SignalR.Client;

partial public class HubConnection
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Trace,
            "Preparing non-blocking invocation of '{Target}', with {ArgumentCount} argument(s).",
            EventName = "PreparingNonBlockingInvocation"
        )]
        partial public static void PreparingNonBlockingInvocation(
            ILogger logger,
            string target,
            int argumentCount
        );

        [LoggerMessage(
            2,
            LogLevel.Trace,
            "Preparing blocking invocation '{InvocationId}' of '{Target}', with return type '{ReturnType}' and {ArgumentCount} argument(s).",
            EventName = "PreparingBlockingInvocation"
        )]
        partial public static void PreparingBlockingInvocation(
            ILogger logger,
            string invocationId,
            string target,
            string returnType,
            int argumentCount
        );

        [LoggerMessage(
            3,
            LogLevel.Debug,
            "Registering Invocation ID '{InvocationId}' for tracking.",
            EventName = "RegisteringInvocation"
        )]
        partial public static void RegisteringInvocation(ILogger logger, string invocationId);

        [LoggerMessage(
            4,
            LogLevel.Trace,
            "Issuing Invocation '{InvocationId}': {ReturnType} {MethodName}({Args}).",
            EventName = "IssuingInvocation",
            SkipEnabledCheck = true
        )]
        partial private static void IssuingInvocation(
            ILogger logger,
            string invocationId,
            string returnType,
            string methodName,
            string args
        );

        public static void IssuingInvocation(
            ILogger logger,
            string invocationId,
            string returnType,
            string methodName,
            object?[] args
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var argsList =
                    args == null
                        ? string.Empty
                        : string.Join(", ", args.Select(a => a?.GetType().FullName ?? "(null)"));
                IssuingInvocation(logger, invocationId, returnType, methodName, argsList);
            }
        }

        public static void SendingMessage(ILogger logger, HubMessage message)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                if (message is HubInvocationMessage invocationMessage)
                {
                    SendingMessage(logger, message.GetType().Name, invocationMessage.InvocationId);
                }
                else
                {
                    SendingMessageGeneric(logger, message.GetType().Name);
                }
            }
        }

        [LoggerMessage(
            5,
            LogLevel.Debug,
            "Sending {MessageType} message '{InvocationId}'.",
            EventName = "SendingMessage",
            SkipEnabledCheck = true
        )]
        partial private static void SendingMessage(
            ILogger logger,
            string messageType,
            string? invocationId
        );

        [LoggerMessage(
            59,
            LogLevel.Debug,
            "Sending {MessageType} message.",
            EventName = "SendingMessageGeneric",
            SkipEnabledCheck = true
        )]
        partial private static void SendingMessageGeneric(ILogger logger, string messageType);

        public static void MessageSent(ILogger logger, HubMessage message)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                if (message is HubInvocationMessage invocationMessage)
                {
                    MessageSent(logger, message.GetType().Name, invocationMessage.InvocationId);
                }
                else
                {
                    MessageSentGeneric(logger, message.GetType().Name);
                }
            }
        }

        [LoggerMessage(
            6,
            LogLevel.Debug,
            "Sending {MessageType} message '{InvocationId}' completed.",
            EventName = "MessageSent",
            SkipEnabledCheck = true
        )]
        partial private static void MessageSent(
            ILogger logger,
            string messageType,
            string? invocationId
        );

        [LoggerMessage(
            60,
            LogLevel.Debug,
            "Sending {MessageType} message completed.",
            EventName = "MessageSentGeneric",
            SkipEnabledCheck = true
        )]
        partial private static void MessageSentGeneric(ILogger logger, string messageType);

        [LoggerMessage(
            7,
            LogLevel.Error,
            "Sending Invocation '{InvocationId}' failed.",
            EventName = "FailedToSendInvocation"
        )]
        partial public static void FailedToSendInvocation(
            ILogger logger,
            string invocationId,
            Exception exception
        );

        [LoggerMessage(
            8,
            LogLevel.Trace,
            "Received Invocation '{InvocationId}': {MethodName}({Args}).",
            EventName = "ReceivedInvocation",
            SkipEnabledCheck = true
        )]
        partial private static void ReceivedInvocation(
            ILogger logger,
            string? invocationId,
            string methodName,
            string args
        );

        public static void ReceivedInvocation(
            ILogger logger,
            string? invocationId,
            string methodName,
            object?[] args
        )
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var argsList =
                    args == null
                        ? string.Empty
                        : string.Join(", ", args.Select(a => a?.GetType().FullName ?? "(null)"));
                ReceivedInvocation(logger, invocationId, methodName, argsList);
            }
        }

        [LoggerMessage(
            9,
            LogLevel.Warning,
            "Dropped unsolicited Completion message for invocation '{InvocationId}'.",
            EventName = "DroppedCompletionMessage"
        )]
        partial public static void DroppedCompletionMessage(ILogger logger, string invocationId);

        [LoggerMessage(
            10,
            LogLevel.Warning,
            "Dropped unsolicited StreamItem message for invocation '{InvocationId}'.",
            EventName = "DroppedStreamMessage"
        )]
        partial public static void DroppedStreamMessage(ILogger logger, string invocationId);

        [LoggerMessage(
            11,
            LogLevel.Trace,
            "Shutting down connection.",
            EventName = "ShutdownConnection"
        )]
        partial public static void ShutdownConnection(ILogger logger);

        [LoggerMessage(
            12,
            LogLevel.Error,
            "Connection is shutting down due to an error.",
            EventName = "ShutdownWithError"
        )]
        partial public static void ShutdownWithError(ILogger logger, Exception exception);

        [LoggerMessage(
            13,
            LogLevel.Trace,
            "Removing pending invocation {InvocationId}.",
            EventName = "RemovingInvocation"
        )]
        partial public static void RemovingInvocation(ILogger logger, string invocationId);

        [LoggerMessage(
            14,
            LogLevel.Warning,
            "Failed to find handler for '{Target}' method.",
            EventName = "MissingHandler"
        )]
        partial public static void MissingHandler(ILogger logger, string target);

        [LoggerMessage(
            15,
            LogLevel.Trace,
            "Received StreamItem for Invocation {InvocationId}.",
            EventName = "ReceivedStreamItem"
        )]
        partial public static void ReceivedStreamItem(ILogger logger, string invocationId);

        [LoggerMessage(
            16,
            LogLevel.Trace,
            "Canceling dispatch of StreamItem message for Invocation {InvocationId}. The invocation was canceled.",
            EventName = "CancelingStreamItem"
        )]
        partial public static void CancelingStreamItem(ILogger logger, string invocationId);

        [LoggerMessage(
            17,
            LogLevel.Warning,
            "Invocation {InvocationId} received stream item after channel was closed.",
            EventName = "ReceivedStreamItemAfterClose"
        )]
        partial public static void ReceivedStreamItemAfterClose(
            ILogger logger,
            string invocationId
        );

        [LoggerMessage(
            18,
            LogLevel.Trace,
            "Received Completion for Invocation {InvocationId}.",
            EventName = "ReceivedInvocationCompletion"
        )]
        partial public static void ReceivedInvocationCompletion(
            ILogger logger,
            string invocationId
        );

        [LoggerMessage(
            19,
            LogLevel.Trace,
            "Canceling dispatch of Completion message for Invocation {InvocationId}. The invocation was canceled.",
            EventName = "CancelingInvocationCompletion"
        )]
        partial public static void CancelingInvocationCompletion(
            ILogger logger,
            string invocationId
        );

        [LoggerMessage(21, LogLevel.Debug, "HubConnection stopped.", EventName = "Stopped")]
        partial public static void Stopped(ILogger logger);

        [LoggerMessage(
            22,
            LogLevel.Critical,
            "Invocation ID '{InvocationId}' is already in use.",
            EventName = "InvocationAlreadyInUse"
        )]
        partial public static void InvocationAlreadyInUse(ILogger logger, string invocationId);

        [LoggerMessage(
            23,
            LogLevel.Error,
            "Unsolicited response received for invocation '{InvocationId}'.",
            EventName = "ReceivedUnexpectedResponse"
        )]
        partial public static void ReceivedUnexpectedResponse(ILogger logger, string invocationId);

        [LoggerMessage(
            24,
            LogLevel.Information,
            "Using HubProtocol '{Protocol} v{Version}'.",
            EventName = "HubProtocol"
        )]
        partial public static void HubProtocol(ILogger logger, string protocol, int version);

        [LoggerMessage(
            25,
            LogLevel.Trace,
            "Preparing streaming invocation '{InvocationId}' of '{Target}', with return type '{ReturnType}' and {ArgumentCount} argument(s).",
            EventName = "PreparingStreamingInvocation"
        )]
        partial public static void PreparingStreamingInvocation(
            ILogger logger,
            string invocationId,
            string target,
            string returnType,
            int argumentCount
        );

        [LoggerMessage(
            26,
            LogLevel.Trace,
            "Resetting keep-alive timer, received a message from the server.",
            EventName = "ResettingKeepAliveTimer"
        )]
        partial public static void ResettingKeepAliveTimer(ILogger logger);

        [LoggerMessage(
            27,
            LogLevel.Error,
            "An exception was thrown in the handler for the Closed event.",
            EventName = "ErrorDuringClosedEvent"
        )]
        partial public static void ErrorDuringClosedEvent(ILogger logger, Exception exception);

        [LoggerMessage(
            28,
            LogLevel.Debug,
            "Sending Hub Handshake.",
            EventName = "SendingHubHandshake"
        )]
        partial public static void SendingHubHandshake(ILogger logger);

        [LoggerMessage(31, LogLevel.Trace, "Received a ping message.", EventName = "ReceivedPing")]
        partial public static void ReceivedPing(ILogger logger);

        [LoggerMessage(
            34,
            LogLevel.Error,
            "Invoking client side method '{MethodName}' failed.",
            EventName = "ErrorInvokingClientSideMethod"
        )]
        partial public static void ErrorInvokingClientSideMethod(
            ILogger logger,
            string methodName,
            Exception exception
        );

        [LoggerMessage(
            35,
            LogLevel.Error,
            "The underlying connection closed while processing the handshake response. See exception for details.",
            EventName = "ErrorReceivingHandshakeResponse"
        )]
        partial public static void ErrorReceivingHandshakeResponse(
            ILogger logger,
            Exception exception
        );

        [LoggerMessage(
            36,
            LogLevel.Error,
            "Server returned handshake error: {Error}",
            EventName = "HandshakeServerError"
        )]
        partial public static void HandshakeServerError(ILogger logger, string error);

        [LoggerMessage(37, LogLevel.Debug, "Received close message.", EventName = "ReceivedClose")]
        partial public static void ReceivedClose(ILogger logger);

        [LoggerMessage(
            38,
            LogLevel.Error,
            "Received close message with an error: {Error}",
            EventName = "ReceivedCloseWithError"
        )]
        partial public static void ReceivedCloseWithError(ILogger logger, string error);

        [LoggerMessage(
            39,
            LogLevel.Debug,
            "Handshake with server complete.",
            EventName = "HandshakeComplete"
        )]
        partial public static void HandshakeComplete(ILogger logger);

        [LoggerMessage(
            40,
            LogLevel.Debug,
            "Registering handler for client method '{MethodName}'.",
            EventName = "RegisteringHandler"
        )]
        partial public static void RegisteringHandler(ILogger logger, string methodName);

        [LoggerMessage(
            58,
            LogLevel.Debug,
            "Removing handlers for client method '{MethodName}'.",
            EventName = "RemovingHandlers"
        )]
        partial public static void RemovingHandlers(ILogger logger, string methodName);

        [LoggerMessage(41, LogLevel.Debug, "Starting HubConnection.", EventName = "Starting")]
        partial public static void Starting(ILogger logger);

        [LoggerMessage(
            43,
            LogLevel.Error,
            "Error starting connection.",
            EventName = "ErrorStartingConnection"
        )]
        partial public static void ErrorStartingConnection(ILogger logger, Exception ex);

        [LoggerMessage(44, LogLevel.Information, "HubConnection started.", EventName = "Started")]
        partial public static void Started(ILogger logger);

        [LoggerMessage(
            45,
            LogLevel.Debug,
            "Sending Cancellation for Invocation '{InvocationId}'.",
            EventName = "SendingCancellation"
        )]
        partial public static void SendingCancellation(ILogger logger, string invocationId);

        [LoggerMessage(
            46,
            LogLevel.Debug,
            "Canceling all outstanding invocations.",
            EventName = "CancelingOutstandingInvocations"
        )]
        partial public static void CancelingOutstandingInvocations(ILogger logger);

        [LoggerMessage(
            47,
            LogLevel.Debug,
            "Receive loop starting.",
            EventName = "ReceiveLoopStarting"
        )]
        partial public static void ReceiveLoopStarting(ILogger logger);

        [LoggerMessage(
            48,
            LogLevel.Debug,
            "Starting server timeout timer. Duration: {ServerTimeout:0.00}ms",
            EventName = "StartingServerTimeoutTimer",
            SkipEnabledCheck = true
        )]
        partial public static void StartingServerTimeoutTimer(ILogger logger, double serverTimeout);

        public static void StartingServerTimeoutTimer(ILogger logger, TimeSpan serverTimeout)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                StartingServerTimeoutTimer(logger, serverTimeout.TotalMilliseconds);
            }
        }

        [LoggerMessage(
            49,
            LogLevel.Debug,
            "Not using server timeout because the transport inherently tracks server availability.",
            EventName = "NotUsingServerTimeout"
        )]
        partial public static void NotUsingServerTimeout(ILogger logger);

        [LoggerMessage(
            50,
            LogLevel.Error,
            "The server connection was terminated with an error.",
            EventName = "ServerDisconnectedWithError"
        )]
        partial public static void ServerDisconnectedWithError(ILogger logger, Exception ex);

        [LoggerMessage(
            51,
            LogLevel.Debug,
            "Invoking the Closed event handler.",
            EventName = "InvokingClosedEventHandler"
        )]
        partial public static void InvokingClosedEventHandler(ILogger logger);

        [LoggerMessage(52, LogLevel.Debug, "Stopping HubConnection.", EventName = "Stopping")]
        partial public static void Stopping(ILogger logger);

        [LoggerMessage(
            53,
            LogLevel.Debug,
            "Terminating receive loop.",
            EventName = "TerminatingReceiveLoop"
        )]
        partial public static void TerminatingReceiveLoop(ILogger logger);

        [LoggerMessage(
            54,
            LogLevel.Debug,
            "Waiting for the receive loop to terminate.",
            EventName = "WaitingForReceiveLoopToTerminate"
        )]
        partial public static void WaitingForReceiveLoopToTerminate(ILogger logger);

        [LoggerMessage(
            56,
            LogLevel.Debug,
            "Processing {MessageLength} byte message from server.",
            EventName = "ProcessingMessage"
        )]
        partial public static void ProcessingMessage(ILogger logger, long messageLength);

        [LoggerMessage(
            42,
            LogLevel.Trace,
            "Waiting on Connection Lock in {MethodName} ({FilePath}:{LineNumber}).",
            EventName = "WaitingOnConnectionLock"
        )]
        partial public static void WaitingOnConnectionLock(
            ILogger logger,
            string? methodName,
            string? filePath,
            int lineNumber
        );

        [LoggerMessage(
            20,
            LogLevel.Trace,
            "Releasing Connection Lock in {MethodName} ({FilePath}:{LineNumber}).",
            EventName = "ReleasingConnectionLock"
        )]
        partial public static void ReleasingConnectionLock(
            ILogger logger,
            string? methodName,
            string? filePath,
            int lineNumber
        );

        [LoggerMessage(
            55,
            LogLevel.Trace,
            "Unable to send cancellation for invocation '{InvocationId}'. The connection is inactive.",
            EventName = "UnableToSendCancellation"
        )]
        partial public static void UnableToSendCancellation(ILogger logger, string invocationId);

        [LoggerMessage(
            57,
            LogLevel.Error,
            "Failed to bind arguments received in invocation '{InvocationId}' of '{MethodName}'.",
            EventName = "ArgumentBindingFailure"
        )]
        partial public static void ArgumentBindingFailure(
            ILogger logger,
            string? invocationId,
            string methodName,
            Exception exception
        );

        [LoggerMessage(
            61,
            LogLevel.Trace,
            "Acquired the Connection Lock in order to ping the server.",
            EventName = "AcquiredConnectionLockForPing"
        )]
        partial public static void AcquiredConnectionLockForPing(ILogger logger);

        [LoggerMessage(
            62,
            LogLevel.Trace,
            "Skipping ping because a send is already in progress.",
            EventName = "UnableToAcquireConnectionLockForPing"
        )]
        partial public static void UnableToAcquireConnectionLockForPing(ILogger logger);

        [LoggerMessage(
            63,
            LogLevel.Trace,
            "Initiating stream '{StreamId}'.",
            EventName = "StartingStream"
        )]
        partial public static void StartingStream(ILogger logger, string streamId);

        [LoggerMessage(
            64,
            LogLevel.Trace,
            "Sending item for stream '{StreamId}'.",
            EventName = "StreamItemSent"
        )]
        partial public static void SendingStreamItem(ILogger logger, string streamId);

        [LoggerMessage(
            65,
            LogLevel.Trace,
            "Stream '{StreamId}' has been canceled by client.",
            EventName = "CancelingStream"
        )]
        partial public static void CancelingStream(ILogger logger, string streamId);

        [LoggerMessage(
            66,
            LogLevel.Trace,
            "Sending completion message for stream '{StreamId}'.",
            EventName = "CompletingStream"
        )]
        partial public static void CompletingStream(ILogger logger, string streamId);

        [LoggerMessage(
            67,
            LogLevel.Error,
            "The HubConnection failed to transition from the {ExpectedState} state to the {NewState} state because it was actually in the {ActualState} state.",
            EventName = "StateTransitionFailed"
        )]
        partial public static void StateTransitionFailed(
            ILogger logger,
            HubConnectionState expectedState,
            HubConnectionState newState,
            HubConnectionState actualState
        );

        [LoggerMessage(
            68,
            LogLevel.Information,
            "HubConnection reconnecting.",
            EventName = "Reconnecting"
        )]
        partial public static void Reconnecting(ILogger logger);

        [LoggerMessage(
            69,
            LogLevel.Error,
            "HubConnection reconnecting due to an error.",
            EventName = "ReconnectingWithError"
        )]
        partial public static void ReconnectingWithError(ILogger logger, Exception exception);

        [LoggerMessage(
            70,
            LogLevel.Information,
            "HubConnection reconnected successfully after {ReconnectAttempts} attempts and {ElapsedTime} elapsed.",
            EventName = "Reconnected"
        )]
        partial public static void Reconnected(
            ILogger logger,
            long reconnectAttempts,
            TimeSpan elapsedTime
        );

        [LoggerMessage(
            71,
            LogLevel.Information,
            "Reconnect retries have been exhausted after {ReconnectAttempts} failed attempts and {ElapsedTime} elapsed. Disconnecting.",
            EventName = "ReconnectAttemptsExhausted"
        )]
        partial public static void ReconnectAttemptsExhausted(
            ILogger logger,
            long reconnectAttempts,
            TimeSpan elapsedTime
        );

        [LoggerMessage(
            72,
            LogLevel.Trace,
            "Reconnect attempt number {ReconnectAttempts} will start in {RetryDelay}.",
            EventName = "AwaitingReconnectRetryDelay"
        )]
        partial public static void AwaitingReconnectRetryDelay(
            ILogger logger,
            long reconnectAttempts,
            TimeSpan retryDelay
        );

        [LoggerMessage(
            73,
            LogLevel.Trace,
            "Reconnect attempt failed.",
            EventName = "ReconnectAttemptFailed"
        )]
        partial public static void ReconnectAttemptFailed(ILogger logger, Exception exception);

        [LoggerMessage(
            74,
            LogLevel.Error,
            "An exception was thrown in the handler for the Reconnecting event.",
            EventName = "ErrorDuringReconnectingEvent"
        )]
        partial public static void ErrorDuringReconnectingEvent(
            ILogger logger,
            Exception exception
        );

        [LoggerMessage(
            75,
            LogLevel.Error,
            "An exception was thrown in the handler for the Reconnected event.",
            EventName = "ErrorDuringReconnectedEvent"
        )]
        partial public static void ErrorDuringReconnectedEvent(ILogger logger, Exception exception);

        [LoggerMessage(
            76,
            LogLevel.Error,
            $"An exception was thrown from {nameof(IRetryPolicy)}.{nameof(IRetryPolicy.NextRetryDelay)}().",
            EventName = "ErrorDuringNextRetryDelay"
        )]
        partial public static void ErrorDuringNextRetryDelay(ILogger logger, Exception exception);

        [LoggerMessage(
            77,
            LogLevel.Warning,
            "Connection not reconnecting because the IRetryPolicy returned null on the first reconnect attempt.",
            EventName = "FirstReconnectRetryDelayNull"
        )]
        partial public static void FirstReconnectRetryDelayNull(ILogger logger);

        [LoggerMessage(
            78,
            LogLevel.Trace,
            "Connection stopped during reconnect delay. Done reconnecting.",
            EventName = "ReconnectingStoppedDueToStateChangeDuringRetryDelay"
        )]
        partial public static void ReconnectingStoppedDuringRetryDelay(ILogger logger);

        [LoggerMessage(
            79,
            LogLevel.Trace,
            "Connection stopped during reconnect attempt. Done reconnecting.",
            EventName = "ReconnectingStoppedDueToStateChangeDuringReconnectAttempt"
        )]
        partial public static void ReconnectingStoppedDuringReconnectAttempt(ILogger logger);

        [LoggerMessage(
            80,
            LogLevel.Trace,
            "The HubConnection is attempting to transition from the {ExpectedState} state to the {NewState} state.",
            EventName = "AttemptingStateTransition"
        )]
        partial public static void AttemptingStateTransition(
            ILogger logger,
            HubConnectionState expectedState,
            HubConnectionState newState
        );

        [LoggerMessage(
            81,
            LogLevel.Error,
            "Received an invalid handshake response.",
            EventName = "ErrorInvalidHandshakeResponse"
        )]
        partial public static void ErrorInvalidHandshakeResponse(
            ILogger logger,
            Exception exception
        );

        public static void ErrorHandshakeTimedOut(
            ILogger logger,
            TimeSpan handshakeTimeout,
            Exception exception
        ) => ErrorHandshakeTimedOut(logger, handshakeTimeout.TotalSeconds, exception);

        [LoggerMessage(
            82,
            LogLevel.Error,
            "The handshake timed out after {HandshakeTimeoutSeconds} seconds.",
            EventName = "ErrorHandshakeTimedOut"
        )]
        partial private static void ErrorHandshakeTimedOut(
            ILogger logger,
            double HandshakeTimeoutSeconds,
            Exception exception
        );

        [LoggerMessage(
            83,
            LogLevel.Error,
            "The handshake was canceled by the client.",
            EventName = "ErrorHandshakeCanceled"
        )]
        partial public static void ErrorHandshakeCanceled(ILogger logger, Exception exception);

        [LoggerMessage(
            84,
            LogLevel.Trace,
            "Client threw an error for stream '{StreamId}'.",
            EventName = "ErroredStream"
        )]
        partial public static void ErroredStream(
            ILogger logger,
            string streamId,
            Exception exception
        );

        [LoggerMessage(
            85,
            LogLevel.Warning,
            "Failed to find a value returning handler for '{Target}' method. Sending error to server.",
            EventName = "MissingResultHandler"
        )]
        partial public static void MissingResultHandler(ILogger logger, string target);

        [LoggerMessage(
            86,
            LogLevel.Warning,
            "Result given for '{Target}' method but server is not expecting a result.",
            EventName = "ResultNotExpected"
        )]
        partial public static void ResultNotExpected(ILogger logger, string target);

        [LoggerMessage(
            87,
            LogLevel.Trace,
            "Completion message for stream '{StreamId}' was not sent because the connection is closed.",
            EventName = "CompletingStreamNotSent"
        )]
        partial public static void CompletingStreamNotSent(ILogger logger, string streamId);

        [LoggerMessage(
            88,
            LogLevel.Warning,
            "Error returning result for invocation '{InvocationId}' for method '{Target}' because the underlying connection is closed.",
            EventName = "ErrorSendingInvocationResult"
        )]
        partial public static void ErrorSendingInvocationResult(
            ILogger logger,
            string invocationId,
            string target,
            Exception exception
        );
    }
}
