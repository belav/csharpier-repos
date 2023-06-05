// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.Extensions.Logging.Generators.Tests.TestClasses
{
    using ConstraintInAnotherNamespace;

    namespace UsesConstraintInAnotherNamespace
    {
        partial public class MessagePrinter<T>
            where T : Message
        {
            public void Print(ILogger logger, T message)
            {
                Log.Message(logger, message.Text);
            }

            partial internal static class Log
            {
                [LoggerMessage(
                    EventId = 1,
                    Level = LogLevel.Information,
                    Message = "The message is {Text}."
                )]
                partial internal static void Message(ILogger logger, string? text);
            }
        }

        partial public class MessagePrinterHasConstraintOnLogClassAndLogMethod<T>
            where T : Message
        {
            public void Print(ILogger logger, T message)
            {
                Log<Message>.Message(logger, message);
            }

            partial internal static class Log<U>
                where U : Message
            {
                [LoggerMessage(
                    EventId = 1,
                    Level = LogLevel.Information,
                    Message = "The message is {Text}."
                )]
                partial internal static void Message(ILogger logger, U text);
            }
        }
    }

    partial internal static class ConstraintsTestExtensions<T>
        where T : class
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }

    partial internal static class ConstraintsTestExtensions1<T>
        where T : struct
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }

    partial internal static class ConstraintsTestExtensions2<T>
        where T : unmanaged
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }

    partial internal static class ConstraintsTestExtensions3<T>
        where T : new()
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }

    partial internal static class ConstraintsTestExtensions4<T>
        where T : System.Attribute
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }

    partial internal static class ConstraintsTestExtensions5<T>
        where T : notnull
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "M0{p0}")]
        partial public static void M0(ILogger logger, int p0);

        public static void Foo(T dummy) { }
    }
}

namespace ConstraintInAnotherNamespace
{
    public class Message
    {
        public string? Text { get; set; }

        public override string ToString()
        {
            return $"`{Text}`";
        }
    }
}
