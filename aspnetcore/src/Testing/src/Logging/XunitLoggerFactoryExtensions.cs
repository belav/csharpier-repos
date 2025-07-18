﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging;

public static class XunitLoggerFactoryExtensions
{
    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper output)
    {
        builder.Services.AddSingleton<ILoggerProvider>(new XunitLoggerProvider(output));
        return builder;
    }

    public static ILoggingBuilder AddXunit(
        this ILoggingBuilder builder,
        ITestOutputHelper output,
        LogLevel minLevel
    )
    {
        builder.Services.AddSingleton<ILoggerProvider>(new XunitLoggerProvider(output, minLevel));
        return builder;
    }

    public static ILoggingBuilder AddXunit(
        this ILoggingBuilder builder,
        ITestOutputHelper output,
        LogLevel minLevel,
        DateTimeOffset? logStart
    )
    {
        builder.Services.AddSingleton<ILoggerProvider>(
            new XunitLoggerProvider(output, minLevel, logStart)
        );
        return builder;
    }

    public static ILoggerFactory AddXunit(
        this ILoggerFactory loggerFactory,
        ITestOutputHelper output
    )
    {
        loggerFactory.AddProvider(new XunitLoggerProvider(output));
        return loggerFactory;
    }

    public static ILoggerFactory AddXunit(
        this ILoggerFactory loggerFactory,
        ITestOutputHelper output,
        LogLevel minLevel
    )
    {
        loggerFactory.AddProvider(new XunitLoggerProvider(output, minLevel));
        return loggerFactory;
    }

    public static ILoggerFactory AddXunit(
        this ILoggerFactory loggerFactory,
        ITestOutputHelper output,
        LogLevel minLevel,
        DateTimeOffset? logStart
    )
    {
        loggerFactory.AddProvider(new XunitLoggerProvider(output, minLevel, logStart));
        return loggerFactory;
    }
}
