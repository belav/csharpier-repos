using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Rewrite.UrlActions;

internal sealed class ForbiddenAction : UrlAction
{
    public override void ApplyAction(
        RewriteContext context,
        BackReferenceCollection? ruleBackReferences,
        BackReferenceCollection? conditionBackReferences
    )
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Result = RuleResult.EndResponse;
    }
}
