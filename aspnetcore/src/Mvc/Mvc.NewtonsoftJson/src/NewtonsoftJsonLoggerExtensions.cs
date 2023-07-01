using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.NewtonsoftJson;

internal static class NewtonsoftJsonLoggerExtensions
{
    private static readonly Action<ILogger, Exception> _jsonInputFormatterException;

    static NewtonsoftJsonLoggerExtensions()
    {
        _jsonInputFormatterException = LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(1, "JsonInputException"),
            "JSON input formatter threw an exception."
        );
    }

    public static void JsonInputException(this ILogger logger, Exception exception)
    {
        _jsonInputFormatterException(logger, exception);
    }
}
