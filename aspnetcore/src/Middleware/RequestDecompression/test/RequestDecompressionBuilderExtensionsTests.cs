using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.RequestDecompression.Tests;

public class RequestDecompressionBuilderExtensionsTests
{
    [Fact]
    public void UseRequestDecompression_NullApplicationBuilder_Throws()
    {
        // Arrange
        IApplicationBuilder builder = null;

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.UseRequestDecompression();
        });
    }
}
