// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Security.Cryptography.Tests
{
    public static class CryptographicExceptionTests
    {
        [Fact]
        public static void Ctor()
        {
            string message = "Some Message";
            var inner = new FormatException(message);

            Assert.NotNull(new CryptographicException().Message);
            Assert.Equal(message, new CryptographicException(message).Message);
            Assert.Equal(message + " 12345", new CryptographicException(message + " {0}", "12345").Message);
            Assert.Equal(5, new CryptographicException(5).HResult);

            Assert.Same(inner, new CryptographicException(message, inner).InnerException);
            Assert.Equal(message, new CryptographicException(message, inner).Message);
        }
    }
}
