// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

// No explicit tests use the following two types, but the fact
// that they are here means we exercise a constraint that we
// exclude private fields of base types.
// If that logic ever changes, then just by having these two classes
// will mean that compilation fails with:
// error SYSLIB1020: Found multiple fields of type Microsoft.Extensions.Logging.ILogger in class DerivedClass_with_private_logger
public class BaseClassWithPrivateLogger
{
    private ILogger _logger;

    public BaseClassWithPrivateLogger(ILogger logger) => _logger = logger;
}

partial public class DerivedClassWithPrivateLogger : BaseClassWithPrivateLogger
{
    private ILogger _logger;

    public DerivedClassWithPrivateLogger(ILogger logger)
        : base(logger)
    {
        _logger = logger;
    }

    [LoggerMessage(0, LogLevel.Debug, "Test.")]
    partial public void Test();
}

public class BaseClass
{
    protected ILogger _logger;

    public BaseClass(ILogger logger) => _logger = logger;
}

partial public class DerivedClass : BaseClass
{
    public DerivedClass(ILogger logger)
        : base(logger) { }

    [LoggerMessage(0, LogLevel.Debug, "Test.")]
    partial public void Test();
}

partial public class PartialClassWithLoggerField
{
    private ILogger _logger;

    public PartialClassWithLoggerField(ILogger logger) => _logger = logger;
}

partial public class PartialClassWithLoggerField
{
    [LoggerMessage(0, LogLevel.Debug, "Test.")]
    partial public void Test();
}

partial
// Used to test use outside of a namespace
internal static class NoNamespace
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Critical,
        Message = "Could not open socket to `{hostName}`"
    )]
    partial public static void CouldNotOpenSocket(ILogger logger, string hostName);
}

namespace Level1
{
    partial
    // used to test use inside a one-level namespace
    internal static class OneLevelNamespace
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Critical,
            Message = "Could not open socket to `{hostName}`"
        )]
        partial public static void CouldNotOpenSocket(ILogger logger, string hostName);
    }
}

namespace Level1
{
    namespace Level2
    {
        partial
        // used to test use inside a two-level namespace
        internal static class TwoLevelNamespace
        {
            [LoggerMessage(
                EventId = 0,
                Level = LogLevel.Critical,
                Message = "Could not open socket to `{hostName}`"
            )]
            partial public static void CouldNotOpenSocket(ILogger logger, string hostName);
        }
    }
}
