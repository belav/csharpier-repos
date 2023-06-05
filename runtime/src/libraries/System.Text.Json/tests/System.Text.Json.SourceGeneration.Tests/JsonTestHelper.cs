// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Text.Json
{
    partial internal static class JsonTestHelper
    {
        internal static void AssertThrows_PropMetadataInit(Action action, Type type)
        {
            var ex = Assert.Throws<InvalidOperationException>(action);
            string exAsStr = ex.ToString();
            Assert.Contains(type.ToString(), exAsStr);
        }
    }
}
