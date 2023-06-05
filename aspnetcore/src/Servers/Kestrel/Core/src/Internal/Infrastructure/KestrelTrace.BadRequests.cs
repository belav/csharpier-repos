// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

partial internal sealed class KestrelTrace : ILogger
{
    public void ConnectionBadRequest(
        string connectionId,
        AspNetCore.Http.BadHttpRequestException ex
    )
    {
        BadRequestsLog.ConnectionBadRequest(_badRequestsLogger, connectionId, ex.Message, ex);
    }

    public void RequestProcessingError(string connectionId, Exception ex)
    {
        BadRequestsLog.RequestProcessingError(_badRequestsLogger, connectionId, ex);
    }

    public void RequestBodyMinimumDataRateNotSatisfied(
        string connectionId,
        string? traceIdentifier,
        double rate
    )
    {
        BadRequestsLog.RequestBodyMinimumDataRateNotSatisfied(
            _badRequestsLogger,
            connectionId,
            traceIdentifier,
            rate
        );
    }

    public void ResponseMinimumDataRateNotSatisfied(string connectionId, string? traceIdentifier)
    {
        BadRequestsLog.ResponseMinimumDataRateNotSatisfied(
            _badRequestsLogger,
            connectionId,
            traceIdentifier
        );
    }

    public void PossibleInvalidHttpVersionDetected(
        string connectionId,
        HttpVersion expectedHttpVersion,
        HttpVersion detectedHttpVersion
    )
    {
        if (_generalLogger.IsEnabled(LogLevel.Debug))
        {
            BadRequestsLog.PossibleInvalidHttpVersionDetected(
                _badRequestsLogger,
                connectionId,
                HttpUtilities.VersionToString(expectedHttpVersion),
                HttpUtilities.VersionToString(detectedHttpVersion)
            );
        }
    }

    partial private static class BadRequestsLog
    {
        [LoggerMessage(
            17,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" bad request data: ""{message}""",
            EventName = "ConnectionBadRequest"
        )]
        partial public static void ConnectionBadRequest(
            ILogger logger,
            string connectionId,
            string message,
            Microsoft.AspNetCore.Http.BadHttpRequestException ex
        );

        [LoggerMessage(
            20,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"" request processing ended abnormally.",
            EventName = "RequestProcessingError"
        )]
        partial public static void RequestProcessingError(
            ILogger logger,
            string connectionId,
            Exception ex
        );

        [LoggerMessage(
            27,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"", Request id ""{TraceIdentifier}"": the request timed out because it was not sent by the client at a minimum of {Rate} bytes/second.",
            EventName = "RequestBodyMinimumDataRateNotSatisfied"
        )]
        partial public static void RequestBodyMinimumDataRateNotSatisfied(
            ILogger logger,
            string connectionId,
            string? traceIdentifier,
            double rate
        );

        [LoggerMessage(
            28,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"", Request id ""{TraceIdentifier}"": the connection was closed because the response was not read by the client at the specified minimum data rate.",
            EventName = "ResponseMinimumDataRateNotSatisfied"
        )]
        partial public static void ResponseMinimumDataRateNotSatisfied(
            ILogger logger,
            string connectionId,
            string? traceIdentifier
        );

        [LoggerMessage(
            54,
            LogLevel.Debug,
            @"Connection id ""{ConnectionId}"": Invalid content received on connection. Possible incorrect HTTP version detected. Expected {ExpectedHttpVersion} but received {DetectedHttpVersion}.",
            EventName = "PossibleInvalidHttpVersionDetected",
            SkipEnabledCheck = true
        )]
        partial public static void PossibleInvalidHttpVersionDetected(
            ILogger logger,
            string connectionId,
            string expectedHttpVersion,
            string detectedHttpVersion
        );

        // Highest shared ID is 63. New consecutive IDs start at 64
    }
}
