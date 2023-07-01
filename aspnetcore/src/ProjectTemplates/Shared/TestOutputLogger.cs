using System.Globalization;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Templates.Test.Helpers;

internal sealed class TestOutputLogger : ITestOutputHelper
{
    private readonly ILogger _logger;

    public TestOutputLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void WriteLine(string message)
    {
        _logger.LogInformation(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, format, args));
    }
}
