using Xunit;

namespace Microsoft.AspNetCore.Razor.Language;

public class DefaultAllowedChildTagDescriptorBuilderTest
{
    [Fact]
    public void Build_DisplayNameIsName()
    {
        // Arrange
        var builder = new DefaultAllowedChildTagDescriptorBuilder(null);
        builder.Name = "foo";

        // Act
        var descriptor = builder.Build();

        // Assert
        Assert.Equal("foo", descriptor.DisplayName);
    }
}
