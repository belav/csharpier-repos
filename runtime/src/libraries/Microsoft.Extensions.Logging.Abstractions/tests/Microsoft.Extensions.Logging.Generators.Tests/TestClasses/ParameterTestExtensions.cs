// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    partial internal static class ParameterTestExtensions
    {
        internal struct S
        {
            public override string ToString() => "Hello from S";
        }

        [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "UseInParameter {s}")]
        partial internal static void UseInParameter(ILogger logger, in S s);

        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "UseRefParameter {s}")]
        partial internal static void UseRefParameter(ILogger logger, ref S s);
    }
}
