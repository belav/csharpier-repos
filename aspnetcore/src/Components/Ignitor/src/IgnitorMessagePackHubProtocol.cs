using Microsoft.AspNetCore.SignalR.Protocol;

namespace Ignitor;

public class IgnitorMessagePackHubProtocol : MessagePackHubProtocol, IHubProtocol
{
    string IHubProtocol.Name => "blazorpack";
}
