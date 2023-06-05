// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;

namespace Internal.NativeCrypto
{
    partial internal static class CapiHelper
    {
        public static CryptographicException GetBadDataException()
        {
            const int NTE_BAD_DATA = unchecked((int)CryptKeyError.NTE_BAD_DATA);
            return new CryptographicException(NTE_BAD_DATA);
        }

        public static CryptographicException GetEFailException()
        {
            return new CryptographicException(E_FAIL);
        }
    }
}
