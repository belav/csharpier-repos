// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

partial internal static class LoggerExtensions
{
    private const string BadDeveloperCertificateStateMessage =
        "The ASP.NET Core developer certificate is in an invalid state. To fix this issue, run the following commands "
        + "'dotnet dev-certs https --clean' and 'dotnet dev-certs https' to remove all existing ASP.NET Core development certificates and create a new untrusted developer certificate. "
        + "On macOS or Windows, use 'dotnet dev-certs https --trust' to trust the new certificate.";

    [LoggerMessage(
        0,
        LogLevel.Debug,
        "Using development certificate: {certificateSubjectName} (Thumbprint: {certificateThumbprint})",
        EventName = "LocatedDevelopmentCertificate"
    )]
    partial private static void LocatedDevelopmentCertificate(
        this ILogger<KestrelServer> logger,
        string certificateSubjectName,
        string certificateThumbprint
    );

    public static void LocatedDevelopmentCertificate(
        this ILogger<KestrelServer> logger,
        X509Certificate2 certificate
    ) => LocatedDevelopmentCertificate(logger, certificate.Subject, certificate.Thumbprint);

    [LoggerMessage(
        1,
        LogLevel.Debug,
        "Unable to locate an appropriate development https certificate.",
        EventName = "UnableToLocateDevelopmentCertificate"
    )]
    partial public static void UnableToLocateDevelopmentCertificate(
        this ILogger<KestrelServer> logger
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Failed to locate the development https certificate at '{certificatePath}'.",
        EventName = "FailedToLocateDevelopmentCertificateFile"
    )]
    partial public static void FailedToLocateDevelopmentCertificateFile(
        this ILogger<KestrelServer> logger,
        string certificatePath
    );

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "Failed to load the development https certificate at '{certificatePath}'.",
        EventName = "FailedToLoadDevelopmentCertificate"
    )]
    partial public static void FailedToLoadDevelopmentCertificate(
        this ILogger<KestrelServer> logger,
        string certificatePath
    );

    [LoggerMessage(
        4,
        LogLevel.Error,
        BadDeveloperCertificateStateMessage,
        EventName = "BadDeveloperCertificateState"
    )]
    partial public static void BadDeveloperCertificateState(this ILogger<KestrelServer> logger);

    [LoggerMessage(5, LogLevel.Warning, "{Message}", EventName = "DeveloperCertificateFirstRun")]
    partial public static void DeveloperCertificateFirstRun(
        this ILogger<KestrelServer> logger,
        string message
    );

    [LoggerMessage(
        6,
        LogLevel.Error,
        "The certificate file at '{CertificateFilePath}' can not be found, contains malformed data or does not contain a certificate.",
        EventName = "MissingOrInvalidCertificateFile"
    )]
    partial public static void FailedToLoadCertificate(
        this ILogger<KestrelServer> logger,
        string certificateFilePath
    );

    [LoggerMessage(
        7,
        LogLevel.Error,
        "The certificate key file at '{CertificateKeyFilePath}' can not be found, contains malformed data or does not contain a PEM encoded key in PKCS8 format.",
        EventName = "MissingOrInvalidCertificateKeyFile"
    )]
    partial public static void FailedToLoadCertificateKey(
        this ILogger<KestrelServer> logger,
        string certificateKeyFilePath
    );

    [LoggerMessage(
        8,
        LogLevel.Warning,
        "The ASP.NET Core developer certificate is not trusted. For information about trusting the ASP.NET Core developer certificate, see https://aka.ms/aspnet/https-trust-dev-cert.",
        EventName = "DeveloperCertificateNotTrusted"
    )]
    partial public static void DeveloperCertificateNotTrusted(this ILogger<KestrelServer> logger);
}
