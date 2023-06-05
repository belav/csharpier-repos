// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Security.Authentication;

partial internal static class Interop
{
    partial internal static class AndroidCrypto
    {
        [LibraryImport(
            Interop.Libraries.AndroidCryptoNative,
            EntryPoint = "AndroidCryptoNative_SSLGetSupportedProtocols"
        )]
        partial internal static SslProtocols SSLGetSupportedProtocols();

        [LibraryImport(
            Libraries.AndroidCryptoNative,
            EntryPoint = "AndroidCryptoNative_SSLSupportsApplicationProtocolsConfiguration"
        )]
        [return: MarshalAs(UnmanagedType.U1)]
        partial internal static bool SSLSupportsApplicationProtocolsConfiguration();
    }
}
