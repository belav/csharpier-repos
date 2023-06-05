// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.IIS.Core;

partial internal abstract class IISHttpContext
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            @"Connection ID ""{ConnectionId}"" disconnecting.",
            EventName = "ConnectionDisconnect"
        )]
        partial public static void ConnectionDisconnect(ILogger logger, string connectionId);

        [LoggerMessage(
            2,
            LogLevel.Error,
            @"Connection ID ""{ConnectionId}"", Request ID ""{TraceIdentifier}"": An unhandled exception was thrown by the application.",
            EventName = "ApplicationError"
        )]
        partial public static void ApplicationError(
            ILogger logger,
            string connectionId,
            string traceIdentifier,
            Exception ex
        );

        [LoggerMessage(
            3,
            LogLevel.Error,
            @"Unexpected exception in ""{ClassName}.{MethodName}"".",
            EventName = "UnexpectedError"
        )]
        partial public static void UnexpectedError(
            ILogger logger,
            string className,
            Exception ex,
            [CallerMemberName] string? methodName = null
        );

        public static void ConnectionBadRequest(
            ILogger logger,
            string connectionId,
            Microsoft.AspNetCore.Http.BadHttpRequestException ex
        ) => ConnectionBadRequest(logger, connectionId, ex.Message, ex);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" bad request data: ""{message}""",
            EventName = nameof(ConnectionBadRequest)
        )]
        partial private static void ConnectionBadRequest(
            ILogger logger,
            string connectionId,
            string message,
            Microsoft.AspNetCore.Http.BadHttpRequestException ex
        );
    }
}
