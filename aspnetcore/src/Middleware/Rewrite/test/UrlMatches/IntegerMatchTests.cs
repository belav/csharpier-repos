﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.UrlMatches;

namespace Microsoft.AspNetCore.Rewrite.Tests.UrlMatches;

public class IntegerMatchTests
{
    [Fact]
    public void IntegerMatch_Constructor_Integer_Parse_Excetion()
    {
        var ex = Assert.Throws<FormatException>(
            () => new IntegerMatch("Not an int", IntegerOperationType.Equal)
        );
        Assert.Equal(ex.Message, Resources.Error_IntegerMatch_FormatExceptionMessage);
    }

    [Theory]
    [InlineData(1, (int)IntegerOperationType.Equal, "1", true)]
    [InlineData(1, (int)IntegerOperationType.NotEqual, "2", true)]
    [InlineData(2, (int)IntegerOperationType.Less, "1", true)]
    [InlineData(1, (int)IntegerOperationType.LessEqual, "2", false)]
    [InlineData(1, (int)IntegerOperationType.Greater, "2", true)]
    [InlineData(2, (int)IntegerOperationType.GreaterEqual, "1", false)]
    [InlineData(1, (int)IntegerOperationType.Equal, "Not an int", false)]
    [InlineData(1, (int)IntegerOperationType.Equal, "", false)]
    [InlineData(1, (int)IntegerOperationType.Equal, "2147483648", false)]
    public void IntegerMatch_Evaluation_Cases_Tests(
        int value,
        int operation,
        string input,
        bool expectedResult
    )
    {
        var context = new RewriteContext { HttpContext = new DefaultHttpContext() };
        var integerMatch = new IntegerMatch(value, (IntegerOperationType)operation);
        var matchResult = integerMatch.Evaluate(input, context);
        Assert.Equal(expectedResult, matchResult.Success);
    }
}
