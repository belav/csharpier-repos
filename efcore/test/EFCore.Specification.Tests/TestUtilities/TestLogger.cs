﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestLogger : TestLoggerBase, IDiagnosticsLogger, ILogger
    {
        public TestLogger(LoggingDefinitions definitions)
        {
            Definitions = definitions;
        }

        public ILoggingOptions Options
            => new LoggingOptions();

        public bool ShouldLogSensitiveData()
            => false;

        public ILogger Logger
            => this;

        public virtual LoggingDefinitions Definitions { get; }

        public IInterceptors Interceptors { get; }

        public bool IsEnabled(LogLevel logLevel)
            => EnabledFor == logLevel;

        public IDisposable BeginScope<TState>(TState state)
            => null;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            LoggedEvent = eventId;
            LoggedAt = logLevel;
            Assert.Equal(LoggedEvent, eventId);
            Message = formatter(state, exception);
        }
    }
}
