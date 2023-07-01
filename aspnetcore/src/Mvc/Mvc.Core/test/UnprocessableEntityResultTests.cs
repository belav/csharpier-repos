using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

public class UnprocessableEntityResultTests
{
    [Fact]
    public void UnprocessableEntityResult_InitializesStatusCode()
    {
        // Arrange & act
        var result = new UnprocessableEntityResult();

        // Assert
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
    }
}
