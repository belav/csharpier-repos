using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Hosting.Fakes;

public class CustomLoggerFactory : ILoggerFactory
{
    public void CustomConfigureMethod() { }

    public void AddProvider(ILoggerProvider provider) { }

    public ILogger CreateLogger(string categoryName) => NullLogger.Instance;

    public void Dispose() { }
}

public class SubLoggerFactory : CustomLoggerFactory { }

public class NonSubLoggerFactory : ILoggerFactory
{
    public void CustomConfigureMethod() { }

    public void AddProvider(ILoggerProvider provider) { }

    public ILogger CreateLogger(string categoryName) => NullLogger.Instance;

    public void Dispose() { }
}
