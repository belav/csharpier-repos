using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Microsoft.AspNetCore.Mvc;

public class EmptyResultTests
{
    [Fact]
    public void EmptyResult_ExecuteResult_IsANoOp()
    {
        // Arrange
        var emptyResult = new EmptyResult();

        var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();

        var context = new ActionContext(httpContext.Object, routeData, actionDescriptor);

        // Act & Assert (does not throw)
        emptyResult.ExecuteResult(context);
    }
}
