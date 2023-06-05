// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_GetNonCryptographicallySecureRandomBytes"
        )]
        partial internal static unsafe void GetNonCryptographicallySecureRandomBytes(
            byte* buffer,
            int length
        );

        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_GetCryptographicallySecureRandomBytes"
        )]
        partial internal static unsafe int GetCryptographicallySecureRandomBytes(
            byte* buffer,
            int length
        );
    }

    internal static unsafe void GetRandomBytes(byte* buffer, int length)
    {
        Sys.GetNonCryptographicallySecureRandomBytes(buffer, length);
    }

    internal static unsafe void GetCryptographicallySecureRandomBytes(byte* buffer, int length)
    {
        if (Sys.GetCryptographicallySecureRandomBytes(buffer, length) != 0)
            throw new CryptographicException();
    }
}
