using System;

namespace Microsoft.Extensions.Logging.Testing;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly,
    AllowMultiple = false
)]
public class LogLevelAttribute : Attribute
{
    public LogLevelAttribute(LogLevel logLevel)
    {
        LogLevel = logLevel;
    }

    public LogLevel LogLevel { get; }
}
