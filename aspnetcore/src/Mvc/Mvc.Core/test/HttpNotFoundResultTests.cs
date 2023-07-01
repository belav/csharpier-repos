using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

public class HttpNotFoundResultTests
{
    [Fact]
    public void HttpNotFoundResult_InitializesStatusCode()
    {
        // Arrange & act
        var notFound = new NotFoundResult();

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
    }
}
