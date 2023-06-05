// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Localization;

partial internal static class RequestCultureProviderLoggerExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "{requestCultureProvider} returned the following unsupported cultures '{cultures}'.",
        EventName = "UnsupportedCulture"
    )]
    partial public static void UnsupportedCultures(
        this ILogger logger,
        string requestCultureProvider,
        IList<StringSegment> cultures
    );

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "{requestCultureProvider} returned the following unsupported UI Cultures '{uiCultures}'.",
        EventName = "UnsupportedUICulture"
    )]
    partial public static void UnsupportedUICultures(
        this ILogger logger,
        string requestCultureProvider,
        IList<StringSegment> uiCultures
    );
}
