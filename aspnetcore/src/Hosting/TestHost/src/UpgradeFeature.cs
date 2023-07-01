using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.AspNetCore.TestHost;

internal sealed class UpgradeFeature : IHttpUpgradeFeature
{
    public bool IsUpgradableRequest => false;

    // TestHost provides an IHttpWebSocketFeature so it wont call UpgradeAsync()
    public Task<Stream> UpgradeAsync()
    {
        throw new NotSupportedException();
    }
}
