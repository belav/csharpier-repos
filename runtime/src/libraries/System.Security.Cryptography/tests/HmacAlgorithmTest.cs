// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Security.Cryptography.Tests
{
    public class HmacAlgorithmTest
    {
        [Fact]
        public void SetNullAlgorithmName()
        {
            using (HMAC hmac = new TestHMAC())
            {
                Assert.Throws<ArgumentNullException>(() => hmac.HashName = null);
                Assert.Null(hmac.HashName);
            }
        }

        [Fact]
        public void SetUnknownAlgorithmName()
        {
            using (HMAC hmac = new TestHMAC())
            {
                const string UnknownAlgorithmName = "No known algorithm name has spaces, so this better be invalid...";

                // Assert.NoThrows is implicit
                hmac.HashName = UnknownAlgorithmName;

                Assert.Equal(UnknownAlgorithmName, hmac.HashName);
            }
        }

        [Fact]
        public void ResetAlgorithmName()
        {
            using (HMAC hmac = new TestHMAC())
            {
                hmac.HashName = "SHA1";

                // On .NET Framework builds this next line will succeed (modulo FIPS prohibitions on MD5).
                // On .NET Core builds it throws an exception.
                Assert.Throws<PlatformNotSupportedException>(() => hmac.HashName = "MD5");
                Assert.Equal("SHA1", hmac.HashName);
            }
        }

        [Fact]
        public void ReassignAlgorithmName()
        {
            using (HMAC hmac = new TestHMAC())
            {
                hmac.HashName = "SHA1";

                hmac.HashName = hmac.HashName;

                // Set it again using a guaranteed-to-be-new copy to ensure it is
                // permissive based on value equality semantics
                hmac.HashName = new string(new[] { 'S', 'H', 'A', '1' });

                Assert.Equal("SHA1", hmac.HashName);
            }
        }

        [Fact]
        public void TrivialDerivationThrows()
        {
            using (HMAC hmac = new TestHMAC())
            {
                hmac.HashName = "SHA1";
                hmac.Key = Array.Empty<byte>();

                byte[] ignored;
                Assert.Throws<PlatformNotSupportedException>(() => ignored = hmac.ComputeHash(Array.Empty<byte>()));
            }
        }

        private class TestHMAC : HMAC
        {
        }
    }
}
