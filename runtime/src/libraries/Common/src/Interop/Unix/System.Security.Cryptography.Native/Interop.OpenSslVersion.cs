// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class OpenSsl
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OpenSslVersionNumber")]
        partial internal static long OpenSslVersionNumber();
    }
}
