using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.RequestDecompression.Tests;

public class RequestDecompressionServiceExtensionsTests
{
    [Fact]
    public void AddRequestDecompression_NullServiceCollection_Throws()
    {
        // Arrange
        IServiceCollection serviceCollection = null;
        var configureOptions = (RequestDecompressionOptions options) => { };

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            serviceCollection.AddRequestDecompression(configureOptions);
        });
    }

    [Fact]
    public void AddRequestDecompression_NullConfigureOptions_Throws()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        Action<RequestDecompressionOptions> configureOptions = null;

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            serviceCollection.AddRequestDecompression(configureOptions);
        });
    }
}
