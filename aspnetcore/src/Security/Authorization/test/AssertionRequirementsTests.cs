// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Xunit;

namespace Microsoft.AspNetCore.Authorization.Test;

public class AssertionRequirementsTests
{
    private AssertionRequirement CreateRequirement()
    {
        return new AssertionRequirement(context => true);
    }

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
