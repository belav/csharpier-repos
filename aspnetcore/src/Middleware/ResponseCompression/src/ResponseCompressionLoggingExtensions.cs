// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.ResponseCompression;

partial internal static class ResponseCompressionLoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "No response compression available, the Accept-Encoding header is missing or invalid.",
        EventName = "NoAcceptEncoding"
    )]
    partial public static void NoAcceptEncoding(this ILogger logger);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "No response compression available for HTTPS requests. See ResponseCompressionOptions.EnableForHttps.",
        EventName = "NoCompressionForHttps"
    )]
    partial public static void NoCompressionForHttps(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Trace,
        "This request accepts compression.",
        EventName = "RequestAcceptsCompression"
    )]
    partial public static void RequestAcceptsCompression(this ILogger logger);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Response compression disabled due to the {header} header.",
        EventName = "NoCompressionDueToHeader"
    )]
    partial public static void NoCompressionDueToHeader(this ILogger logger, string header);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Response compression is not enabled for the Content-Type '{header}'.",
        EventName = "NoCompressionForContentType"
    )]
    partial public static void NoCompressionForContentType(this ILogger logger, string header);

    [LoggerMessage(
        6,
        LogLevel.Trace,
        "Response compression is available for this Content-Type.",
        EventName = "ShouldCompressResponse"
    )]
    partial public static void ShouldCompressResponse(this ILogger logger);

    [LoggerMessage(
        7,
        LogLevel.Debug,
        "No matching response compression provider found.",
        EventName = "NoCompressionProvider"
    )]
    partial public static void NoCompressionProvider(this ILogger logger);

    [LoggerMessage(
        8,
        LogLevel.Debug,
        "The response will be compressed with '{provider}'.",
        EventName = "CompressWith"
    )]
    partial public static void CompressingWith(this ILogger logger, string provider);
}
