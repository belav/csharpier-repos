// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Interop.Libraries.Crypt32, SetLastError = true)]
        partial internal static IntPtr CertEnumCertificatesInStore(
            SafeCertStoreHandle hCertStore,
            IntPtr pPrevCertContext
        );
    }
}
