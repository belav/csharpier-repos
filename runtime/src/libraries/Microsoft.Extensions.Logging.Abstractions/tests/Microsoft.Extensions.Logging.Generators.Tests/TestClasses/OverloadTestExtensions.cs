// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    partial internal static class OverloadTestExtensions
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        [LoggerMessage(EventId = 1, Level = LogLevel.Trace, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, string p0);
    }
}
