// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(
            Libraries.Crypt32,
            EntryPoint = "CryptDecodeObject",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CryptDecodeObjectPointer(
            CertEncodingType dwCertEncodingType,
            IntPtr lpszStructType,
            byte[] pbEncoded,
            int cbEncoded,
            CryptDecodeObjectFlags dwFlags,
            void* pvStructInfo,
            ref int pcbStructInfo
        );

        [LibraryImport(
            Libraries.Crypt32,
            EntryPoint = "CryptDecodeObject",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CryptDecodeObjectPointer(
            CertEncodingType dwCertEncodingType,
            IntPtr lpszStructType,
            byte* pbEncoded,
            int cbEncoded,
            CryptDecodeObjectFlags dwFlags,
            void* pvStructInfo,
            ref int pcbStructInfo
        );
    }
}
