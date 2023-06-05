// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Grpc.JsonTranscoding.Internal;

partial internal static class GrpcServerLog
{
    [LoggerMessage(
        1,
        LogLevel.Information,
        "Request content-type of '{ContentType}' is not supported.",
        EventName = "UnsupportedRequestContentType"
    )]
    partial public static void UnsupportedRequestContentType(ILogger logger, string? contentType);

    [LoggerMessage(
        2,
        LogLevel.Error,
        "Error when executing service method '{ServiceMethod}'.",
        EventName = "ErrorExecutingServiceMethod"
    )]
    partial public static void ErrorExecutingServiceMethod(
        ILogger logger,
        string serviceMethod,
        Exception ex
    );

    [LoggerMessage(
        3,
        LogLevel.Information,
        "Error status code '{StatusCode}' with detail '{Detail}' raised.",
        EventName = "RpcConnectionError"
    )]
    partial public static void RpcConnectionError(
        ILogger logger,
        StatusCode statusCode,
        string detail,
        Exception? debugException
    );

    [LoggerMessage(4, LogLevel.Debug, "Reading message.", EventName = "ReadingMessage")]
    partial public static void ReadingMessage(ILogger logger);

    [LoggerMessage(
        5,
        LogLevel.Trace,
        "Deserializing to '{MessageType}'.",
        EventName = "DeserializingMessage"
    )]
    partial public static void DeserializingMessage(ILogger logger, Type messageType);

    [LoggerMessage(6, LogLevel.Trace, "Received message.", EventName = "ReceivedMessage")]
    partial public static void ReceivedMessage(ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Information,
        "Error reading message.",
        EventName = "ErrorReadingMessage"
    )]
    partial public static void ErrorReadingMessage(ILogger logger, Exception ex);

    [LoggerMessage(8, LogLevel.Debug, "Sending message.", EventName = "SendingMessage")]
    partial public static void SendingMessage(ILogger logger);

    [LoggerMessage(9, LogLevel.Debug, "Message sent.", EventName = "MessageSent")]
    partial public static void MessageSent(ILogger logger);

    [LoggerMessage(
        10,
        LogLevel.Information,
        "Error sending message.",
        EventName = "ErrorSendingMessage"
    )]
    partial public static void ErrorSendingMessage(ILogger logger, Exception ex);

    [LoggerMessage(
        11,
        LogLevel.Trace,
        "Serialized '{MessageType}'.",
        EventName = "SerializedMessage"
    )]
    partial public static void SerializedMessage(ILogger logger, Type messageType);
}
