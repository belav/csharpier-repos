// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeyGetDsa")]
        partial internal static SafeDsaHandle EvpPkeyGetDsa(SafeEvpPKeyHandle pkey);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeySetDsa")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool EvpPkeySetDsa(SafeEvpPKeyHandle pkey, SafeDsaHandle key);
    }
}
