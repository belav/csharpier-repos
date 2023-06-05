// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        internal enum CryptGetKeyParamFlags : int
        {
            CRYPT_EXPORT = 0x0004,
            KP_IV = 1,
            KP_PERMISSIONS = 6,
            KP_ALGID = 7,
            KP_KEYLEN = 9
        }

        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static bool CryptGetKeyParam(
            SafeCapiKeyHandle hKey,
            CryptGetKeyParamFlags dwParam,
            byte[]? pbData,
            ref int pdwDataLen,
            int dwFlags
        );
    }
}
