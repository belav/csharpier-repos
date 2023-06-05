// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CryptImportPublicKeyInfoEx2(
            CertEncodingType dwCertEncodingType,
            CERT_PUBLIC_KEY_INFO* pInfo,
            CryptImportPublicKeyInfoFlags dwFlags,
            void* pvAuxInfo,
            out SafeBCryptKeyHandle phKey
        );
    }
}
