using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.UrlActions;

namespace Microsoft.AspNetCore.Rewrite.Tests.UrlActions;

public class ForbiddenActionTests
{
    [Fact]
    public void Forbidden_Verify403IsInStatusCode()
    {
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        var action = new ForbiddenAction();

        action.ApplyAction(context, null, null);

        Assert.Equal(RuleResult.EndResponse, context.Result);
        Assert.Equal(StatusCodes.Status403Forbidden, context.HttpContext.Response.StatusCode);
    }
}
