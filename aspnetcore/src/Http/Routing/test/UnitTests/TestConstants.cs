using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Routing;

public static class TestConstants
{
    internal static readonly RequestDelegate EmptyRequestDelegate = (context) => Task.CompletedTask;
}
