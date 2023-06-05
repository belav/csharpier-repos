// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Ssl
    {
        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_SslCtxSetProtocolOptions"
        )]
        partial internal static void SslCtxSetProtocolOptions(IntPtr ctx, SslProtocols protocols);

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_SslCtxSetProtocolOptions"
        )]
        partial internal static void SslCtxSetProtocolOptions(
            SafeSslContextHandle ctx,
            SslProtocols protocols
        );
    }
}
