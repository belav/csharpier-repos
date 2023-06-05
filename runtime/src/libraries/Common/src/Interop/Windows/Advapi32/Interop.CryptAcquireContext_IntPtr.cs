// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "CryptAcquireContextW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool CryptAcquireContext(
            out IntPtr psafeProvHandle,
            char* pszContainer,
            char* pszProvider,
            int dwProvType,
            Interop.Crypt32.CryptAcquireContextFlags dwFlags
        );
    }
}
