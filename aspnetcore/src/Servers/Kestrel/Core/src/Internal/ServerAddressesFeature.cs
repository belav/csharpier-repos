using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

internal sealed class ServerAddressesFeature : IServerAddressesFeature
{
    public ServerAddressesCollection InternalCollection { get; } = new ServerAddressesCollection();

    ICollection<string> IServerAddressesFeature.Addresses => InternalCollection.PublicCollection;
    public bool PreferHostingUrls { get; set; }
}
