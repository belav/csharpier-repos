using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.UrlActions;

namespace Microsoft.AspNetCore.Rewrite.Tests.UrlActions;

public class AbortActionTests
{
    public void AbortAction_VerifyEndResponseResult()
    {
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        var action = new AbortAction();

        action.ApplyAction(context, null, null);

        Assert.Equal(RuleResult.EndResponse, context.Result);
    }
}
