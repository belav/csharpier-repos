using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Microsoft.AspNetCore.Authorization.Test;

public class OperationAuthorizationRequirementTests
{
    private OperationAuthorizationRequirement CreateRequirement(string name)
    {
        return new OperationAuthorizationRequirement() { Name = name };
    }

    [Fact]
    public void ToString_ShouldReturnFormatValue()
    {
        // Arrange
        var requirement = CreateRequirement("Custom");

        // Act
        var formattedValue = requirement.ToString();

        // Assert
        Assert.Equal("OperationAuthorizationRequirement:Name=Custom", formattedValue);
    }
}
