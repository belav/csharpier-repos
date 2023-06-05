// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static bool CryptSetKeyParam(
            SafeCapiKeyHandle hKey,
            int dwParam,
            byte[] pbData,
            int dwFlags
        );

        [LibraryImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial public static bool CryptSetKeyParam(
            SafeCapiKeyHandle safeKeyHandle,
            int dwParam,
            ref int pdw,
            int dwFlags
        );
    }
}
