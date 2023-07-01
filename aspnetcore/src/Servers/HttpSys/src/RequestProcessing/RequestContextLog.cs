using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

internal static partial class RequestContextLog
{
    [LoggerMessage(
        LoggerEventIds.RequestError,
        LogLevel.Error,
        "ProcessRequestAsync",
        EventName = "RequestError"
    )]
    public static partial void RequestError(ILogger logger, Exception exception);

    [LoggerMessage(
        LoggerEventIds.RequestProcessError,
        LogLevel.Error,
        "ProcessRequestAsync",
        EventName = "RequestProcessError"
    )]
    public static partial void RequestProcessError(ILogger logger, Exception exception);

    [LoggerMessage(
        LoggerEventIds.RequestsDrained,
        LogLevel.Information,
        "All requests drained.",
        EventName = "RequestsDrained"
    )]
    public static partial void RequestsDrained(ILogger logger);
}
