using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.StackTrace.Sources;

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        0,
        LogLevel.Debug,
        "Failed to read stack trace information for exception.",
        EventName = "FailedToReadStackTraceInfo"
    )]
    public static partial void FailedToReadStackTraceInfo(this ILogger logger, Exception exception);
}
