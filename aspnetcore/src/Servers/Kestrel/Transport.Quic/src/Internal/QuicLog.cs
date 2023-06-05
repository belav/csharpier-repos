// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Security;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Quic.Internal;

partial internal static class QuicLog
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        @"Connection id ""{ConnectionId}"" accepted.",
        EventName = "AcceptedConnection",
        SkipEnabledCheck = true
    )]
    partial private static void AcceptedConnectionCore(ILogger logger, string connectionId);

    public static void AcceptedConnection(ILogger logger, BaseConnectionContext connection)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            AcceptedConnectionCore(logger, connection.ConnectionId);
        }
    }

    [LoggerMessage(
        2,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" type {StreamType} accepted.",
        EventName = "AcceptedStream",
        SkipEnabledCheck = true
    )]
    partial private static void AcceptedStreamCore(
        ILogger logger,
        string connectionId,
        StreamType streamType
    );

    public static void AcceptedStream(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            AcceptedStreamCore(logger, streamContext.ConnectionId, GetStreamType(streamContext));
        }
    }

    [LoggerMessage(
        3,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" type {StreamType} connected.",
        EventName = "ConnectedStream",
        SkipEnabledCheck = true
    )]
    partial private static void ConnectedStreamCore(
        ILogger logger,
        string connectionId,
        StreamType streamType
    );

    public static void ConnectedStream(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            ConnectedStreamCore(logger, streamContext.ConnectionId, GetStreamType(streamContext));
        }
    }

    [LoggerMessage(
        4,
        LogLevel.Debug,
        @"Connection id ""{ConnectionId}"" unexpected error.",
        EventName = "ConnectionError",
        SkipEnabledCheck = true
    )]
    partial private static void ConnectionErrorCore(
        ILogger logger,
        string connectionId,
        Exception ex
    );

    public static void ConnectionError(
        ILogger logger,
        BaseConnectionContext connection,
        Exception ex
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            ConnectionErrorCore(logger, connection.ConnectionId, ex);
        }
    }

    [LoggerMessage(
        5,
        LogLevel.Debug,
        @"Connection id ""{ConnectionId}"" aborted by peer with error code {ErrorCode}.",
        EventName = "ConnectionAborted",
        SkipEnabledCheck = true
    )]
    partial private static void ConnectionAbortedCore(
        ILogger logger,
        string connectionId,
        long errorCode,
        Exception ex
    );

    public static void ConnectionAborted(
        ILogger logger,
        BaseConnectionContext connection,
        long errorCode,
        Exception ex
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            ConnectionAbortedCore(logger, connection.ConnectionId, errorCode, ex);
        }
    }

    [LoggerMessage(
        6,
        LogLevel.Debug,
        @"Connection id ""{ConnectionId}"" aborted by application with error code {ErrorCode} because: ""{Reason}"".",
        EventName = "ConnectionAbort",
        SkipEnabledCheck = true
    )]
    partial private static void ConnectionAbortCore(
        ILogger logger,
        string connectionId,
        long errorCode,
        string reason
    );

    public static void ConnectionAbort(
        ILogger logger,
        BaseConnectionContext connection,
        long errorCode,
        string reason
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            ConnectionAbortCore(logger, connection.ConnectionId, errorCode, reason);
        }
    }

    [LoggerMessage(
        7,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" unexpected error.",
        EventName = "StreamError",
        SkipEnabledCheck = true
    )]
    partial private static void StreamErrorCore(ILogger logger, string connectionId, Exception ex);

    public static void StreamError(ILogger logger, QuicStreamContext streamContext, Exception ex)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamErrorCore(logger, streamContext.ConnectionId, ex);
        }
    }

    [LoggerMessage(
        8,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" paused.",
        EventName = "StreamPause",
        SkipEnabledCheck = true
    )]
    partial private static void StreamPauseCore(ILogger logger, string connectionId);

    public static void StreamPause(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamPauseCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        9,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" resumed.",
        EventName = "StreamResume",
        SkipEnabledCheck = true
    )]
    partial private static void StreamResumeCore(ILogger logger, string connectionId);

    public static void StreamResume(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamResumeCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        10,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" shutting down writes because: ""{Reason}"".",
        EventName = "StreamShutdownWrite",
        SkipEnabledCheck = true
    )]
    partial private static void StreamShutdownWriteCore(
        ILogger logger,
        string connectionId,
        string reason
    );

    public static void StreamShutdownWrite(
        ILogger logger,
        QuicStreamContext streamContext,
        string reason
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamShutdownWriteCore(logger, streamContext.ConnectionId, reason);
        }
    }

    [LoggerMessage(
        11,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" read aborted by peer with error code {ErrorCode}.",
        EventName = "StreamAbortedRead",
        SkipEnabledCheck = true
    )]
    partial private static void StreamAbortedReadCore(
        ILogger logger,
        string connectionId,
        long errorCode
    );

    public static void StreamAbortedRead(
        ILogger logger,
        QuicStreamContext streamContext,
        long errorCode
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamAbortedReadCore(logger, streamContext.ConnectionId, errorCode);
        }
    }

    [LoggerMessage(
        12,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" write aborted by peer with error code {ErrorCode}.",
        EventName = "StreamAbortedWrite",
        SkipEnabledCheck = true
    )]
    partial private static void StreamAbortedWriteCore(
        ILogger logger,
        string connectionId,
        long errorCode
    );

    public static void StreamAbortedWrite(
        ILogger logger,
        QuicStreamContext streamContext,
        long errorCode
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamAbortedWriteCore(logger, streamContext.ConnectionId, errorCode);
        }
    }

    [LoggerMessage(
        13,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" aborted by application with error code {ErrorCode} because: ""{Reason}"".",
        EventName = "StreamAbort",
        SkipEnabledCheck = true
    )]
    partial private static void StreamAbortCore(
        ILogger logger,
        string connectionId,
        long errorCode,
        string reason
    );

    public static void StreamAbort(
        ILogger logger,
        QuicStreamContext streamContext,
        long errorCode,
        string reason
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamAbortCore(logger, streamContext.ConnectionId, errorCode, reason);
        }
    }

    [LoggerMessage(
        14,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" read side aborted by application with error code {ErrorCode} because: ""{Reason}"".",
        EventName = "StreamAbortRead",
        SkipEnabledCheck = true
    )]
    partial private static void StreamAbortReadCore(
        ILogger logger,
        string connectionId,
        long errorCode,
        string reason
    );

    public static void StreamAbortRead(
        ILogger logger,
        QuicStreamContext streamContext,
        long errorCode,
        string reason
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamAbortReadCore(logger, streamContext.ConnectionId, errorCode, reason);
        }
    }

    [LoggerMessage(
        15,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" write side aborted by application with error code {ErrorCode} because: ""{Reason}"".",
        EventName = "StreamAbortWrite",
        SkipEnabledCheck = true
    )]
    partial private static void StreamAbortWriteCore(
        ILogger logger,
        string connectionId,
        long errorCode,
        string reason
    );

    public static void StreamAbortWrite(
        ILogger logger,
        QuicStreamContext streamContext,
        long errorCode,
        string reason
    )
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamAbortWriteCore(logger, streamContext.ConnectionId, errorCode, reason);
        }
    }

    [LoggerMessage(
        16,
        LogLevel.Trace,
        @"Stream id ""{ConnectionId}"" pooled for reuse.",
        EventName = "StreamPooled",
        SkipEnabledCheck = true
    )]
    partial private static void StreamPooledCore(ILogger logger, string connectionId);

    public static void StreamPooled(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            StreamPooledCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        17,
        LogLevel.Trace,
        @"Stream id ""{ConnectionId}"" reused from pool.",
        EventName = "StreamReused",
        SkipEnabledCheck = true
    )]
    partial private static void StreamReusedCore(ILogger logger, string connectionId);

    public static void StreamReused(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            StreamReusedCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        18,
        LogLevel.Warning,
        $"{nameof(SslServerAuthenticationOptions)} must provide a server certificate using {nameof(SslServerAuthenticationOptions.ServerCertificate)},"
            + $" {nameof(SslServerAuthenticationOptions.ServerCertificateContext)}, or {nameof(SslServerAuthenticationOptions.ServerCertificateSelectionCallback)}.",
        EventName = "ConnectionListenerCertificateNotSpecified"
    )]
    partial public static void ConnectionListenerCertificateNotSpecified(ILogger logger);

    [LoggerMessage(
        19,
        LogLevel.Warning,
        $"{nameof(SslServerAuthenticationOptions)} must provide at least one application protocol using {nameof(SslServerAuthenticationOptions.ApplicationProtocols)}.",
        EventName = "ConnectionListenerApplicationProtocolsNotSpecified"
    )]
    partial public static void ConnectionListenerApplicationProtocolsNotSpecified(ILogger logger);

    [LoggerMessage(
        20,
        LogLevel.Debug,
        "QUIC listener starting with configured endpoint {listenEndPoint}.",
        EventName = "ConnectionListenerStarting"
    )]
    partial public static void ConnectionListenerStarting(
        ILogger logger,
        IPEndPoint listenEndPoint
    );

    [LoggerMessage(
        21,
        LogLevel.Debug,
        "QUIC listener aborted.",
        EventName = "ConnectionListenerAborted"
    )]
    partial public static void ConnectionListenerAborted(ILogger logger, Exception exception);

    [LoggerMessage(
        22,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" read timed out.",
        EventName = "StreamTimeoutRead",
        SkipEnabledCheck = true
    )]
    partial private static void StreamTimeoutReadCore(ILogger logger, string connectionId);

    public static void StreamTimeoutRead(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamTimeoutReadCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        23,
        LogLevel.Debug,
        @"Stream id ""{ConnectionId}"" write timed out.",
        EventName = "StreamTimeoutWrite",
        SkipEnabledCheck = true
    )]
    partial private static void StreamTimeoutWriteCore(ILogger logger, string connectionId);

    public static void StreamTimeoutWrite(ILogger logger, QuicStreamContext streamContext)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            StreamTimeoutWriteCore(logger, streamContext.ConnectionId);
        }
    }

    [LoggerMessage(
        24,
        LogLevel.Debug,
        "QUIC listener connection failed.",
        EventName = "ConnectionListenerAcceptConnectionFailed"
    )]
    partial public static void ConnectionListenerAcceptConnectionFailed(
        ILogger logger,
        Exception exception
    );

    private static StreamType GetStreamType(QuicStreamContext streamContext) =>
        streamContext.CanRead && streamContext.CanWrite
            ? StreamType.Bidirectional
            : StreamType.Unidirectional;

    private enum StreamType
    {
        Unidirectional,
        Bidirectional
    }
}
