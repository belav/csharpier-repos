// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_LookupFriendlyNameByOid",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static int LookupFriendlyNameByOid(
            string oidValue,
            ref IntPtr friendlyNamePtr
        );
    }
}
