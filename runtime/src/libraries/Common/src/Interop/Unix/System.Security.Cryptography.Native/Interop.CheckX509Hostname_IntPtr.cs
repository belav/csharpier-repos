// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_CheckX509Hostname",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static int CheckX509Hostname(
            IntPtr x509,
            string hostname,
            int cchHostname
        );
    }
}
