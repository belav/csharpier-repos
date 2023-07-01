using Xunit;

namespace Microsoft.AspNetCore.Http.Result;

public class BadRequestObjectResultTests
{
    [Fact]
    public void BadRequestObjectResult_SetsStatusCodeAndValue()
    {
        // Arrange & Act
        var obj = new object();
        var badRequestObjectResult = new BadRequestObjectResult(obj);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
        Assert.Equal(obj, badRequestObjectResult.Value);
    }
}
