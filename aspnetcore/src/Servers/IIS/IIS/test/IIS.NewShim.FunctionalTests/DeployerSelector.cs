using Microsoft.AspNetCore.Server.IntegrationTesting;

namespace Microsoft.AspNetCore.Server.IIS.FunctionalTests;

public static class DeployerSelector
{
    public static ServerType ServerType => ServerType.IIS;
    public static bool IsNewShimTest => true;
    public static bool HasNewShim => true;
    public static bool HasNewHandler => false;
}
