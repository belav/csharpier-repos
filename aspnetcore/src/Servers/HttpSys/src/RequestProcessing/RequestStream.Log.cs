using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.HttpSys;

internal partial class RequestStream
{
    private static partial class Log
    {
        [LoggerMessage(
            LoggerEventIds.ErrorWhenReadAsync,
            LogLevel.Debug,
            "ReadAsync",
            EventName = "ErrorWhenReadAsync"
        )]
        public static partial void ErrorWhenReadAsync(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ErrorWhenReadBegun,
            LogLevel.Debug,
            "BeginRead",
            EventName = "ErrorWhenReadBegun"
        )]
        public static partial void ErrorWhenReadBegun(ILogger logger, Exception exception);

        [LoggerMessage(
            LoggerEventIds.ErrorWhileRead,
            LogLevel.Debug,
            "Read",
            EventName = "ErrorWhileRead"
        )]
        public static partial void ErrorWhileRead(ILogger logger, Exception exception);
    }
}
