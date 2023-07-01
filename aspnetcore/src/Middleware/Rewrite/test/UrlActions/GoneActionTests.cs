using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.UrlActions;

namespace Microsoft.AspNetCore.Rewrite.Tests.UrlActions;

public class GoneActionTests
{
    [Fact]
    public void Gone_Verify410IsInStatusCode()
    {
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        var action = new GoneAction();

        action.ApplyAction(context, null, null);

        Assert.Equal(RuleResult.EndResponse, context.Result);
        Assert.Equal(StatusCodes.Status410Gone, context.HttpContext.Response.StatusCode);
    }
}
