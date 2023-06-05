// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [LibraryImport(
            Libraries.Crypt32,
            EntryPoint = "CertNameToStrW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int CertNameToStr(
            int dwCertEncodingType,
            void* pName,
            int dwStrType,
            char* psz,
            int csz
        );
    }
}
