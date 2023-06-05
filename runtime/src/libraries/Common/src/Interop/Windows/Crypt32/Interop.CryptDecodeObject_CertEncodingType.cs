// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptDecodeObject(
            CertEncodingType dwCertEncodingType,
            IntPtr lpszStructType,
            byte[] pbEncoded,
            int cbEncoded,
            CryptDecodeObjectFlags dwFlags,
            byte[]? pvStructInfo,
            ref int pcbStructInfo
        );
    }
}
