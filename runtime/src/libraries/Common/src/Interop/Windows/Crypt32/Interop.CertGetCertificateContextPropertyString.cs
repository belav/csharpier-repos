// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(
            Libraries.Crypt32,
            EntryPoint = "CertGetCertificateContextProperty",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CertGetCertificateContextPropertyString(
            SafeCertContextHandle pCertContext,
            CertContextPropId dwPropId,
            byte* pvData,
            ref uint pcbData
        );
    }
}
