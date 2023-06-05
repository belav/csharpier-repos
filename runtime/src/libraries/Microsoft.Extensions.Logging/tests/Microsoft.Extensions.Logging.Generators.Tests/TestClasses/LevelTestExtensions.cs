// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    partial internal static class LevelTestExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "M0")]
        partial public static void M0(ILogger logger);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "M1")]
        partial public static void M1(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "M2")]
        partial public static void M2(ILogger logger);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "M3")]
        partial public static void M3(ILogger logger);

        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "M4")]
        partial public static void M4(ILogger logger);

        [LoggerMessage(EventId = 5, Level = LogLevel.Critical, Message = "M5")]
        partial public static void M5(ILogger logger);

        [LoggerMessage(EventId = 6, Level = LogLevel.None, Message = "M6")]
        partial public static void M6(ILogger logger);

        [LoggerMessage(EventId = 7, Level = (LogLevel)42, Message = "M7")]
        partial public static void M7(ILogger logger);

        [LoggerMessage(EventId = 8, Message = "M8")]
        partial public static void M8(ILogger logger, LogLevel level);

        [LoggerMessage(EventId = 9, Message = "M9")]
        partial public static void M9(LogLevel level, ILogger logger);
    }
}
