// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.SignalR.Internal;

partial internal static class DefaultHubDispatcherLog
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Received hub invocation: {InvocationMessage}.",
        EventName = "ReceivedHubInvocation"
    )]
    partial public static void ReceivedHubInvocation(
        ILogger logger,
        InvocationMessage invocationMessage
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Received unsupported message of type '{MessageType}'.",
        EventName = "UnsupportedMessageReceived"
    )]
    partial public static void UnsupportedMessageReceived(ILogger logger, string messageType);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Unknown hub method '{HubMethod}'.",
        EventName = "UnknownHubMethod"
    )]
    partial public static void UnknownHubMethod(ILogger logger, string hubMethod);

    // 4, OutboundChannelClosed - removed

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Failed to invoke '{HubMethod}' because user is unauthorized.",
        EventName = "HubMethodNotAuthorized"
    )]
    partial public static void HubMethodNotAuthorized(ILogger logger, string hubMethod);

    public static void StreamingResult(
        ILogger logger,
        string invocationId,
        ObjectMethodExecutor objectMethodExecutor
    )
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            var resultType =
                objectMethodExecutor.AsyncResultType ?? objectMethodExecutor.MethodReturnType;
            StreamingResult(logger, invocationId, resultType.FullName);
        }
    }

    [LoggerMessage(
        6,
        LogLevel.Trace,
        "InvocationId {InvocationId}: Streaming result of type '{ResultType}'.",
        EventName = "StreamingResult",
        SkipEnabledCheck = true
    )]
    partial private static void StreamingResult(
        ILogger logger,
        string invocationId,
        string? resultType
    );

    public static void SendingResult(
        ILogger logger,
        string? invocationId,
        ObjectMethodExecutor objectMethodExecutor
    )
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            var resultType =
                objectMethodExecutor.AsyncResultType ?? objectMethodExecutor.MethodReturnType;
            SendingResult(logger, invocationId, resultType.FullName);
        }
    }

    [LoggerMessage(
        7,
        LogLevel.Trace,
        "InvocationId {InvocationId}: Sending result of type '{ResultType}'.",
        EventName = "SendingResult",
        SkipEnabledCheck = true
    )]
    partial private static void SendingResult(
        ILogger logger,
        string? invocationId,
        string? resultType
    );

    [LoggerMessage(
        8,
        LogLevel.Error,
        "Failed to invoke hub method '{HubMethod}'.",
        EventName = "FailedInvokingHubMethod"
    )]
    partial public static void FailedInvokingHubMethod(
        ILogger logger,
        string hubMethod,
        Exception exception
    );

    [LoggerMessage(
        9,
        LogLevel.Trace,
        "'{HubName}' hub method '{HubMethod}' is bound.",
        EventName = "HubMethodBound"
    )]
    partial public static void HubMethodBound(ILogger logger, string hubName, string hubMethod);

    [LoggerMessage(
        10,
        LogLevel.Debug,
        "Canceling stream for invocation {InvocationId}.",
        EventName = "CancelStream"
    )]
    partial public static void CancelStream(ILogger logger, string invocationId);

    [LoggerMessage(
        11,
        LogLevel.Debug,
        "CancelInvocationMessage received unexpectedly.",
        EventName = "UnexpectedCancel"
    )]
    partial public static void UnexpectedCancel(ILogger logger);

    [LoggerMessage(
        12,
        LogLevel.Debug,
        "Received stream hub invocation: {InvocationMessage}.",
        EventName = "ReceivedStreamHubInvocation"
    )]
    partial public static void ReceivedStreamHubInvocation(
        ILogger logger,
        StreamInvocationMessage invocationMessage
    );

    [LoggerMessage(
        13,
        LogLevel.Debug,
        "A streaming method was invoked with a non-streaming invocation : {InvocationMessage}.",
        EventName = "StreamingMethodCalledWithInvoke"
    )]
    partial public static void StreamingMethodCalledWithInvoke(
        ILogger logger,
        HubMethodInvocationMessage invocationMessage
    );

    [LoggerMessage(
        14,
        LogLevel.Debug,
        "A non-streaming method was invoked with a streaming invocation : {InvocationMessage}.",
        EventName = "NonStreamingMethodCalledWithStream"
    )]
    partial public static void NonStreamingMethodCalledWithStream(
        ILogger logger,
        HubMethodInvocationMessage invocationMessage
    );

    [LoggerMessage(
        15,
        LogLevel.Debug,
        "A streaming method returned a value that cannot be used to build enumerator {HubMethod}.",
        EventName = "InvalidReturnValueFromStreamingMethod"
    )]
    partial public static void InvalidReturnValueFromStreamingMethod(
        ILogger logger,
        string hubMethod
    );

    public static void ReceivedStreamItem(ILogger logger, StreamItemMessage message) =>
        ReceivedStreamItem(logger, message.InvocationId);

    [LoggerMessage(
        16,
        LogLevel.Trace,
        "Received item for stream '{StreamId}'.",
        EventName = "ReceivedStreamItem"
    )]
    partial private static void ReceivedStreamItem(ILogger logger, string? streamId);

    [LoggerMessage(
        17,
        LogLevel.Trace,
        "Creating streaming parameter channel '{StreamId}'.",
        EventName = "StartingParameterStream"
    )]
    partial public static void StartingParameterStream(ILogger logger, string streamId);

    public static void CompletingStream(ILogger logger, CompletionMessage message) =>
        CompletingStream(logger, message.InvocationId);

    [LoggerMessage(
        18,
        LogLevel.Trace,
        "Stream '{StreamId}' has been completed by client.",
        EventName = "CompletingStream"
    )]
    partial private static void CompletingStream(ILogger logger, string? streamId);

    public static void ClosingStreamWithBindingError(ILogger logger, CompletionMessage message) =>
        ClosingStreamWithBindingError(logger, message.InvocationId, message.Error);

    [LoggerMessage(
        19,
        LogLevel.Debug,
        "Stream '{StreamId}' closed with error '{Error}'.",
        EventName = "ClosingStreamWithBindingError"
    )]
    partial private static void ClosingStreamWithBindingError(
        ILogger logger,
        string? streamId,
        string? error
    );

    // Retired [20]UnexpectedStreamCompletion, replaced with more generic [24]UnexpectedCompletion

    [LoggerMessage(
        21,
        LogLevel.Debug,
        "StreamItemMessage received unexpectedly.",
        EventName = "UnexpectedStreamItem"
    )]
    partial public static void UnexpectedStreamItem(ILogger logger);

    [LoggerMessage(
        22,
        LogLevel.Debug,
        "Parameters to hub method '{HubMethod}' are incorrect.",
        EventName = "InvalidHubParameters"
    )]
    partial public static void InvalidHubParameters(
        ILogger logger,
        string hubMethod,
        Exception exception
    );

    [LoggerMessage(
        23,
        LogLevel.Debug,
        "Invocation ID '{InvocationId}' is already in use.",
        EventName = "InvocationIdInUse"
    )]
    partial public static void InvocationIdInUse(ILogger logger, string InvocationId);

    [LoggerMessage(
        24,
        LogLevel.Debug,
        "CompletionMessage for invocation ID '{InvocationId}' received unexpectedly.",
        EventName = "UnexpectedCompletion"
    )]
    partial public static void UnexpectedCompletion(ILogger logger, string invocationId);

    [LoggerMessage(
        25,
        LogLevel.Error,
        "Invocation ID {InvocationId}: Failed while sending stream items from hub method {HubMethod}.",
        EventName = "FailedStreaming"
    )]
    partial public static void FailedStreaming(
        ILogger logger,
        string invocationId,
        string hubMethod,
        Exception exception
    );
}
