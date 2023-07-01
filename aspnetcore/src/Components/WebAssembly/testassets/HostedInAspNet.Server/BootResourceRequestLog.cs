using System.Collections.Concurrent;

namespace HostedInAspNet.Server;

public class BootResourceRequestLog
{
    private readonly ConcurrentBag<string> _requestPaths = new ConcurrentBag<string>();

    public IReadOnlyCollection<string> RequestPaths => _requestPaths;

    public void AddRequest(HttpRequest request)
    {
        _requestPaths.Add(request.Path);
    }

    public void Clear()
    {
        _requestPaths.Clear();
    }
}
