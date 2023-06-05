// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    partial internal static class EventNameTestExtensions
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "M0",
            EventName = "CustomEventName"
        )]
        partial public static void M0(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Trace, Message = "CustomEventName")] // EventName inferred from method name
        partial public static void CustomEventName(ILogger logger);
    }
}
