using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

public class HttpUnauthorizedResultTests
{
    [Fact]
    public void HttpUnauthorizedResult_InitializesStatusCode()
    {
        // Arrange & act
        var result = new UnauthorizedResult();

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }
}
