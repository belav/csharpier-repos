using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BasicWebSite;

public class ManagerHandler : AuthorizationHandler<OperationAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement
    )
    {
        if (context.User.HasClaim("Manager", "yes"))
        {
            context.Succeed(requirement);
        }
        return Task.FromResult(0);
    }
}
