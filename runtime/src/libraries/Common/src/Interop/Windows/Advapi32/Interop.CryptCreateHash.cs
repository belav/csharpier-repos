// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [Flags]
        internal enum CryptCreateHashFlags : int
        {
            None = 0,
        }

        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptCreateHash(
            SafeProvHandle hProv,
            int Algid,
            SafeCapiKeyHandle hKey,
            CryptCreateHashFlags dwFlags,
            out SafeHashHandle phHash
        );
    }
}
