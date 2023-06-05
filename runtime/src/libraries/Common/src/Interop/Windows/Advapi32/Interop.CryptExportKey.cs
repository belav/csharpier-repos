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
        partial public static bool CryptExportKey(
            SafeCapiKeyHandle hKey,
            SafeCapiKeyHandle hExpKey,
            int dwBlobType,
            int dwFlags,
            byte[]? pbData,
            ref int dwDataLen
        );
    }
}
