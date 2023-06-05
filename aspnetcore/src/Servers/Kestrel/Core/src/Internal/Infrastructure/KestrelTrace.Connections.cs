// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

partial internal sealed class KestrelTrace : ILogger
{
    public void ConnectionStart(string connectionId)
    {
        ConnectionsLog.ConnectionStart(_connectionsLogger, connectionId);
    }

    public void ConnectionStop(string connectionId)
    {
        ConnectionsLog.ConnectionStop(_connectionsLogger, connectionId);
    }

    public void ConnectionPause(string connectionId)
    {
        ConnectionsLog.ConnectionPause(_connectionsLogger, connectionId);
    }

    public void ConnectionResume(string connectionId)
    {
        ConnectionsLog.ConnectionResume(_connectionsLogger, connectionId);
    }

    public void ConnectionKeepAlive(string connectionId)
    {
        ConnectionsLog.ConnectionKeepAlive(_connectionsLogger, connectionId);
    }

    public void ConnectionDisconnect(string connectionId)
    {
        ConnectionsLog.ConnectionDisconnect(_connectionsLogger, connectionId);
    }

    public void NotAllConnectionsClosedGracefully()
    {
        ConnectionsLog.NotAllConnectionsClosedGracefully(_connectionsLogger);
    }

    public void NotAllConnectionsAborted()
    {
        ConnectionsLog.NotAllConnectionsAborted(_connectionsLogger);
    }

    public void ConnectionRejected(string connectionId)
    {
        ConnectionsLog.ConnectionRejected(_connectionsLogger, connectionId);
    }

    public void ApplicationAbortedConnection(string connectionId, string traceIdentifier)
    {
        ConnectionsLog.ApplicationAbortedConnection(
            _connectionsLogger,
            connectionId,
            traceIdentifier
        );
    }

    public void ConnectionAccepted(string connectionId)
    {
        ConnectionsLog.ConnectionAccepted(_connectionsLogger, connectionId);
    }

    partial private static class ConnectionsLog
    {
        [LoggerMessage(
            1,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" started.",
            EventName = "ConnectionStart"
        )]
        partial public static void ConnectionStart(ILogger logger, string connectionId);

        [LoggerMessage(
            2,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" stopped.",
            EventName = "ConnectionStop"
        )]
        partial public static void ConnectionStop(ILogger logger, string connectionId);

        [LoggerMessage(
            4,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" paused.",
            EventName = "ConnectionPause"
        )]
        partial public static void ConnectionPause(ILogger logger, string connectionId);

        [LoggerMessage(
            5,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" resumed.",
            EventName = "ConnectionResume"
        )]
        partial public static void ConnectionResume(ILogger logger, string connectionId);

        [LoggerMessage(
            9,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" completed keep alive response.",
            EventName = "ConnectionKeepAlive"
        )]
        partial public static void ConnectionKeepAlive(ILogger logger, string connectionId);

        [LoggerMessage(
            10,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" disconnecting.",
            EventName = "ConnectionDisconnect"
        )]
        partial public static void ConnectionDisconnect(ILogger logger, string connectionId);

        [LoggerMessage(
            16,
            LogLevel.Debug,
            "Some connections failed to close gracefully during server shutdown.",
            EventName = "NotAllConnectionsClosedGracefully"
        )]
        partial public static void NotAllConnectionsClosedGracefully(ILogger logger);

        [LoggerMessage(
            21,
            LogLevel.Debug,
            "Some connections failed to abort during server shutdown.",
            EventName = "NotAllConnectionsAborted"
        )]
        partial public static void NotAllConnectionsAborted(ILogger logger);

        [LoggerMessage(
            24,
            LogLevel.Warning,
            @"Connection id ""{ConnectionId}"" rejected because the maximum number of concurrent connections has been reached.",
            EventName = "ConnectionRejected"
        )]
        partial public static void ConnectionRejected(ILogger logger, string connectionId);

        [LoggerMessage(
            34,
            LogLevel.Information,
            @"Connection id ""{ConnectionId}"", Request id ""{TraceIdentifier}"": the application aborted the connection.",
            EventName = "ApplicationAbortedConnection"
        )]
        partial public static void ApplicationAbortedConnection(
            ILogger logger,
            string connectionId,
            string traceIdentifier
        );

        [LoggerMessage(
            39,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" accepted.",
            EventName = "ConnectionAccepted"
        )]
        partial public static void ConnectionAccepted(ILogger logger, string connectionId);

        // Highest shared ID is 63. New consecutive IDs start at 64
    }
}
