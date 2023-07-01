using Microsoft.AspNetCore.SignalR.Common.Tests.Internal.Protocol;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Microsoft.AspNetCore.Components.Server.BlazorPack;

public class BlazorPackHubProtocolTest : MessagePackHubProtocolTestBase
{
    protected override IHubProtocol HubProtocol { get; } = new BlazorPackHubProtocol();
}
