using Microsoft.AspNetCore.Testing;

namespace Microsoft.AspNetCore.Mvc.ModelBinding;

public class ModelMetadataProviderExtensionsTest
{
    [Fact]
    public void GetMetadataForPropertyInvalidPropertyNameThrows()
    {
        // Arrange
        var provider = (IModelMetadataProvider)new EmptyModelMetadataProvider();

        // Act & Assert
        ExceptionAssert.ThrowsArgument(
            () => provider.GetMetadataForProperty(typeof(object), propertyName: "BadPropertyName"),
            "propertyName",
            "The property System.Object.BadPropertyName could not be found."
        );
    }
}
