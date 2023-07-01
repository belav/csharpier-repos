using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

public class BadRequestResultTests
{
    [Fact]
    public void BadRequestResult_InitializesStatusCode()
    {
        // Arrange & act
        var badRequest = new BadRequestResult();

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }
}
