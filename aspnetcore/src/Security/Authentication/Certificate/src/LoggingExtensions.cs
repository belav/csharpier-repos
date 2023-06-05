// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging;

partial internal static class LoggingExtensions
{
    [LoggerMessage(0, LogLevel.Debug, "No client certificate found.", EventName = "NoCertificate")]
    partial public static void NoCertificate(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Not https, skipping certificate authentication.",
        EventName = "NotHttps"
    )]
    partial public static void NotHttps(this ILogger logger);

    [LoggerMessage(
        1,
        LogLevel.Warning,
        "{CertificateType} certificate rejected, subject was {Subject}.",
        EventName = "CertificateRejected"
    )]
    partial public static void CertificateRejected(
        this ILogger logger,
        string certificateType,
        string subject
    );

    [LoggerMessage(
        2,
        LogLevel.Warning,
        "Certificate validation failed, subject was {Subject}. {ChainErrors}",
        EventName = "CertificateFailedValidation"
    )]
    partial public static void CertificateFailedValidation(
        this ILogger logger,
        string subject,
        IList<string> chainErrors
    );
}
