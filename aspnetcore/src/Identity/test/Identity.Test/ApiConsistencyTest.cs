using System.Reflection;

namespace Microsoft.AspNetCore.Identity.Test;

public class ApiConsistencyTest : ApiConsistencyTestBase
{
    protected override Assembly TargetAssembly => typeof(IdentityOptions).Assembly;
}
