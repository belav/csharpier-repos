using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Microsoft.AspNetCore.Authorization.Test;

public class DenyAnonymousAuthorizationRequirementTests
{
    private DenyAnonymousAuthorizationRequirement CreateRequirement()
    {
        return new DenyAnonymousAuthorizationRequirement();
    }

    [Fact]
    public void ToString_ShouldReturnFormatValue()
    {
        // Arrange
        var requirement = CreateRequirement();

        // Act
        var formattedValue = requirement.ToString();

        // Assert
        Assert.Equal(
            "DenyAnonymousAuthorizationRequirement: Requires an authenticated user.",
            formattedValue
        );
    }
}
