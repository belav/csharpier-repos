// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    using NamespaceForABC;

    partial internal static class NestedClassTestsExtensions<T>
        where T : ABC
    {
        partial internal static class NestedMiddleParentClass
        {
            partial internal static class NestedClass
            {
                [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "M8")]
                partial public static void M8(ILogger logger);
            }
        }
    }

    partial internal class NonStaticNestedClassTestsExtensions<T>
        where T : ABC
    {
        partial internal class NonStaticNestedMiddleParentClass
        {
            partial internal static class NestedClass
            {
                [LoggerMessage(EventId = 9, Level = LogLevel.Debug, Message = "M9")]
                partial public static void M9(ILogger logger);
            }
        }
    }

    partial public struct NestedStruct
    {
        partial internal static class Logger
        {
            [LoggerMessage(EventId = 10, Level = LogLevel.Debug, Message = "M10")]
            partial public static void M10(ILogger logger);
        }
    }

    partial public record NestedRecord(string Name, string Address)
    {
        partial internal static class Logger
        {
            [LoggerMessage(EventId = 11, Level = LogLevel.Debug, Message = "M11")]
            partial public static void M11(ILogger logger);
        }
    }

    partial public static class MultiLevelNestedClass
    {
        partial public struct NestedStruct
        {
            partial internal record NestedRecord(string Name, string Address)
            {
                partial internal static class Logger
                {
                    [LoggerMessage(EventId = 12, Level = LogLevel.Debug, Message = "M12")]
                    partial public static void M12(ILogger logger);
                }
            }
        }
    }
}

namespace NamespaceForABC
{
    public class ABC { }
}
