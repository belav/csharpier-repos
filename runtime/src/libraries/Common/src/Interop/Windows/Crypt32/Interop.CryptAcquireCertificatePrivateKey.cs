// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptAcquireCertificatePrivateKey(
            SafeCertContextHandle pCert,
            CryptAcquireCertificatePrivateKeyFlags dwFlags,
            IntPtr pvParameters,
            out IntPtr phCryptProvOrNCryptKey,
            out CryptKeySpec pdwKeySpec,
            [MarshalAs(UnmanagedType.Bool)] out bool pfCallerFreeProvOrNCryptKey
        );
    }
}
