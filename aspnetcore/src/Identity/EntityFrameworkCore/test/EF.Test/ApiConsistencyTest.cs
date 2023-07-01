using System.Reflection;
using Microsoft.AspNetCore.Identity.Test;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test;

public class ApiConsistencyTest : ApiConsistencyTestBase
{
    protected override Assembly TargetAssembly => typeof(IdentityUser).GetTypeInfo().Assembly;
}
