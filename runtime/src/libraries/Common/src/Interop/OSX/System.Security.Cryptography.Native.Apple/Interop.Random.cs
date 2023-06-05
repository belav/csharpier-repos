// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class AppleCrypto
    {
        internal static unsafe void GetRandomBytes(byte* pbBuffer, int count)
        {
            Debug.Assert(count >= 0);

            int errorCode;
            int ret = AppleCryptoNative_GetRandomBytes(pbBuffer, count, &errorCode);

            if (ret == 0)
            {
                throw CreateExceptionForCCError(errorCode, CCRNGStatus);
            }

            if (ret != 1)
            {
                throw new CryptographicException();
            }
        }

        [LibraryImport(Libraries.AppleCryptoNative)]
        partial private static unsafe int AppleCryptoNative_GetRandomBytes(
            byte* buf,
            int num,
            int* errorCode
        );
    }
}
