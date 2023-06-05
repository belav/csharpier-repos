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
            EntryPoint = "CertStrToNameW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CertStrToName(
            CertEncodingType dwCertEncodingType,
            string pszX500,
            CertNameStrTypeAndFlags dwStrType,
            IntPtr pvReserved,
            byte[]? pbEncoded,
            ref int pcbEncoded,
            IntPtr ppszError
        );
    }
}
