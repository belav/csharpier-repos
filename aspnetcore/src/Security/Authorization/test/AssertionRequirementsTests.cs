using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Microsoft.AspNetCore.Authorization.Test;

public class AssertionRequirementsTests
{
    [Fact]
    public void ToString_ShouldReturnFormatValue()
    {
        // Arrange
        var requirement = new AssertionRequirement(context => true);

        // Act
        var formattedValue = requirement.ToString();

        // Assert
        Assert.Equal("Handler assertion should evaluate to true.", formattedValue);
    }
}
