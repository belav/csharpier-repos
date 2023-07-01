using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.SignalR.Tests;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly,
    AllowMultiple = true
)]
public class WebSocketsSupportedConditionAttribute : Attribute, ITestCondition
{
    public bool IsMet => TestHelpers.IsWebSocketsSupported();

    public string SkipReason => "No WebSockets Client for this platform";
}
