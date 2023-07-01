using System.Security.Principal;

namespace Microsoft.AspNetCore.Components.Authorization;

public class TestIdentity : IIdentity
{
    public string AuthenticationType => "Test";

    public bool IsAuthenticated => true;

    public string Name { get; set; }
}
