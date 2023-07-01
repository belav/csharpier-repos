using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BasicWebSite;

public static class Operations
{
    public static OperationAuthorizationRequirement Edit = new OperationAuthorizationRequirement
    {
        Name = "Edit"
    };
}
