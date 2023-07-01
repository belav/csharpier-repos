using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace SecurityWebSite;

public class CountingPolicyEvaluator : PolicyEvaluator
{
    public int AuthorizeCount { get; private set; }

    public CountingPolicyEvaluator(IAuthorizationService authorization)
        : base(authorization) { }

    public override Task<PolicyAuthorizationResult> AuthorizeAsync(
        AuthorizationPolicy policy,
        AuthenticateResult authenticationResult,
        HttpContext context,
        object resource
    )
    {
        AuthorizeCount++;
        return base.AuthorizeAsync(policy, authenticationResult, context, resource);
    }
}
