// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    partial internal static class ExceptionTestExtensions
    {
#pragma warning disable SYSLIB1013
        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "M2 {arg1}: {ex}")]
        partial public static void M2(ILogger logger, string arg1, Exception ex);
#pragma warning disable SYSLIB1013
    }
}
