// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.Logging
{
    partial public static class DebugLoggerFactoryExtensions
    {
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddDebug(
            this Microsoft.Extensions.Logging.ILoggingBuilder builder
        )
        {
            throw null;
        }
    }
}

namespace Microsoft.Extensions.Logging.Debug
{
    [Microsoft.Extensions.Logging.ProviderAliasAttribute("Debug")]
    partial public class DebugLoggerProvider
        : Microsoft.Extensions.Logging.ILoggerProvider,
            System.IDisposable
    {
        public DebugLoggerProvider() { }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
        {
            throw null;
        }

        public void Dispose() { }
    }
}
