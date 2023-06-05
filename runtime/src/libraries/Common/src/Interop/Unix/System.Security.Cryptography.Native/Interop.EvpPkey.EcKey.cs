// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeyGetEcKey")]
        partial internal static SafeEcKeyHandle EvpPkeyGetEcKey(SafeEvpPKeyHandle pkey);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeySetEcKey")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool EvpPkeySetEcKey(SafeEvpPKeyHandle pkey, SafeEcKeyHandle key);
    }
}
