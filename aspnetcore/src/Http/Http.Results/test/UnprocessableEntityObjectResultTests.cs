using Xunit;

namespace Microsoft.AspNetCore.Http.Result;

public class UnprocessableEntityObjectResultTests
{
    [Fact]
    public void UnprocessableEntityObjectResult_SetsStatusCodeAndValue()
    {
        // Arrange & Act
        var obj = new object();
        var result = new UnprocessableEntityObjectResult(obj);

        // Assert
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal(obj, result.Value);
    }
}
