// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if NETCOREAPP
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.Cryptography.KeyDerivation.PBKDF2
{
    /// <summary>
    /// Implements Pbkdf2 using <see cref="Rfc2898DeriveBytes"/>.
    /// </summary>
    internal sealed class NetCorePbkdf2Provider : IPbkdf2Provider
    {
        public byte[] DeriveKey(string password, byte[] salt, KeyDerivationPrf prf, int iterationCount, int numBytesRequested)
        {
            Debug.Assert(password != null);
            Debug.Assert(salt != null);
            Debug.Assert(iterationCount > 0);
            Debug.Assert(numBytesRequested > 0);

            HashAlgorithmName algorithmName;
            switch (prf)
            {
                case KeyDerivationPrf.HMACSHA1:
                    algorithmName = HashAlgorithmName.SHA1;
                    break;
                case KeyDerivationPrf.HMACSHA256:
                    algorithmName = HashAlgorithmName.SHA256;
                    break;
                case KeyDerivationPrf.HMACSHA512:
                    algorithmName = HashAlgorithmName.SHA512;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterationCount, algorithmName, numBytesRequested);
        }
    }
}
#endif
