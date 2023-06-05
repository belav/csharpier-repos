// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

partial internal static class MigrationsEndPointMiddlewareLoggerExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Error,
        "No context type was specified. Ensure the form data from the request includes a 'context' value, specifying the context type name to apply migrations for.",
        EventName = "NoContextType"
    )]
    partial public static void NoContextType(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Error,
        "The context type '{ContextTypeName}' was not found in services. This usually means the context was not registered in services during startup. You probably want to call AddScoped<>() inside the UseServices(...) call in your application startup code.",
        EventName = "ContextNotRegistered"
    )]
    partial public static void ContextNotRegistered(this ILogger logger, string contextTypeName);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Request path matched the path configured for this migrations endpoint({RequestPath}). Attempting to process the migrations request.",
        EventName = "RequestPathMatched"
    )]
    partial public static void RequestPathMatched(this ILogger logger, string requestPath);

    [LoggerMessage(
        5,
        LogLevel.Debug,
        "Request is valid, applying migrations for context '{ContextTypeName}'",
        EventName = "ApplyingMigrations"
    )]
    partial public static void ApplyingMigrations(this ILogger logger, string contextTypeName);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "Migrations successfully applied for context '{ContextTypeName}'.",
        EventName = "MigrationsApplied"
    )]
    partial public static void MigrationsApplied(this ILogger logger, string contextTypeName);

    [LoggerMessage(
        7,
        LogLevel.Error,
        "An error occurred while applying the migrations for '{ContextTypeName}'. See InnerException for details:",
        EventName = "MigrationsEndPointException"
    )]
    partial public static void MigrationsEndPointMiddlewareException(
        this ILogger logger,
        string contextTypeName,
        Exception exception
    );
}

partial internal static class DatabaseErrorPageMiddlewareLoggerExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Debug,
        "{ExceptionType} occurred, checking if Entity Framework recorded this exception as resulting from a failed database operation.",
        EventName = "AttemptingToMatchException"
    )]
    partial public static void AttemptingToMatchException(this ILogger logger, Type exceptionType);

    [LoggerMessage(
        2,
        LogLevel.Debug,
        "Entity Framework did not record any exceptions due to failed database operations. This means the current exception is not a failed Entity Framework database operation, or the current exception occurred from a DbContext that was not obtained from request services.",
        EventName = "NoRecordedException"
    )]
    partial public static void NoRecordedException(this ILogger logger);

    [LoggerMessage(
        3,
        LogLevel.Debug,
        "The current exception (and its inner exceptions) do not match the last exception Entity Framework recorded due to a failed database operation. This means the database operation exception was handled and another exception occurred later in the request.",
        EventName = "NoMatchFound"
    )]
    partial public static void NoMatch(this ILogger logger);

    [LoggerMessage(
        4,
        LogLevel.Debug,
        "Entity Framework recorded that the current exception was due to a failed database operation. Attempting to show database error page.",
        EventName = "MatchFound"
    )]
    partial public static void Matched(this ILogger logger);

    [LoggerMessage(
        6,
        LogLevel.Debug,
        "The target data store is not a relational database. Skipping the database error page.",
        EventName = "NotRelationalDatabase"
    )]
    partial public static void NotRelationalDatabase(this ILogger logger);

    [LoggerMessage(
        5,
        LogLevel.Error,
        "The context type '{ContextTypeName}' was not found in services. This usually means the context was not registered in services during startup. You probably want to call AddScoped<>() inside the UseServices(...) call in your application startup code. Skipping display of the database error page.",
        EventName = "ContextNotRegistered"
    )]
    partial public static void ContextNotRegisteredDatabaseErrorPageMiddleware(
        this ILogger logger,
        string contextTypeName
    );

    [LoggerMessage(
        7,
        LogLevel.Error,
        "An exception occurred while calculating the database error page content. Skipping display of the database error page.",
        EventName = "DatabaseErrorPageException"
    )]
    partial public static void DatabaseErrorPageMiddlewareException(
        this ILogger logger,
        Exception exception
    );
}

partial internal static class DatabaseDeveloperPageExceptionFilterLoggerExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Warning,
        "The response has already started, the next developer page exception filter will not be executed.",
        EventName = "ResponseStarted"
    )]
    partial public static void ResponseStartedDatabaseDeveloperPageExceptionFilter(
        this ILogger logger
    );
}
