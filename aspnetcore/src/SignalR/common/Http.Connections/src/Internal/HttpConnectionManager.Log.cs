// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http.Connections.Internal;

partial internal sealed class HttpConnectionManager
{
    partial private static class Log
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            "New connection {TransportConnectionId} created.",
            EventName = "CreatedNewConnection"
        )]
        partial public static void CreatedNewConnection(
            ILogger logger,
            string transportConnectionId
        );

        [LoggerMessage(
            2,
            LogLevel.Debug,
            "Removing connection {TransportConnectionId} from the list of connections.",
            EventName = "RemovedConnection"
        )]
        partial public static void RemovedConnection(ILogger logger, string transportConnectionId);

        [LoggerMessage(
            3,
            LogLevel.Error,
            "Failed disposing connection {TransportConnectionId}.",
            EventName = "FailedDispose"
        )]
        partial public static void FailedDispose(
            ILogger logger,
            string transportConnectionId,
            Exception exception
        );

        [LoggerMessage(
            5,
            LogLevel.Trace,
            "Connection {TransportConnectionId} timed out.",
            EventName = "ConnectionTimedOut"
        )]
        partial public static void ConnectionTimedOut(ILogger logger, string transportConnectionId);

        [LoggerMessage(
            4,
            LogLevel.Trace,
            "Connection {TransportConnectionId} was reset.",
            EventName = "ConnectionReset"
        )]
        partial public static void ConnectionReset(
            ILogger logger,
            string transportConnectionId,
            Exception exception
        );

        [LoggerMessage(
            7,
            LogLevel.Error,
            "Scanning connections failed.",
            EventName = "ScanningConnectionsFailed"
        )]
        partial public static void ScanningConnectionsFailed(ILogger logger, Exception exception);

        // 8, ScannedConnections - removed

        [LoggerMessage(
            9,
            LogLevel.Trace,
            "Starting connection heartbeat.",
            EventName = "HeartBeatStarted"
        )]
        partial public static void HeartBeatStarted(ILogger logger);

        [LoggerMessage(
            10,
            LogLevel.Trace,
            "Ending connection heartbeat.",
            EventName = "HeartBeatEnded"
        )]
        partial public static void HeartBeatEnded(ILogger logger);

        [LoggerMessage(
            11,
            LogLevel.Debug,
            "Connection {TransportConnectionId} closing because the authentication token has expired.",
            EventName = "AuthenticationExpired"
        )]
        partial public static void AuthenticationExpired(
            ILogger logger,
            string transportConnectionId
        );
    }
}
