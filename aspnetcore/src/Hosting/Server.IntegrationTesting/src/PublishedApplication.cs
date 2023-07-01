using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Server.IntegrationTesting;

public class PublishedApplication : IDisposable
{
    private readonly ILogger _logger;

    public string Path { get; }

    public PublishedApplication(string path, ILogger logger)
    {
        _logger = logger;
        Path = path;
    }

    public void Dispose()
    {
        RetryHelper.RetryOperation(
            () => Directory.Delete(Path, true),
            e => _logger.LogWarning($"Failed to delete directory : {e.Message}"),
            retryCount: 3,
            retryDelayMilliseconds: 100
        );
    }
}
