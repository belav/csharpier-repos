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
        partial internal static bool CertGetCertificateContextProperty(
            SafeCertContextHandle pCertContext,
            CertContextPropId dwPropId,
            byte[]? pvData,
            ref int pcbData
        );

        [LibraryImport(
            Libraries.Crypt32,
            SetLastError = true,
            EntryPoint = "CertGetCertificateContextProperty"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CertGetCertificateContextPropertyPtr(
            SafeCertContextHandle pCertContext,
            CertContextPropId dwPropId,
            byte* pvData,
            ref int pcbData
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertGetCertificateContextProperty(
            SafeCertContextHandle pCertContext,
            CertContextPropId dwPropId,
            out IntPtr pvData,
            ref int pcbData
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertGetCertificateContextProperty(
            SafeCertContextHandle pCertContext,
            CertContextPropId dwPropId,
            out DATA_BLOB pvData,
            ref int pcbData
        );
    }
}
