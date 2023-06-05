// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Ssl
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxAddExtraChainCert")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SslCtxAddExtraChainCert(
            SafeSslContextHandle ctx,
            SafeX509Handle x509
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxUseCertificate")]
        partial internal static int SslCtxUseCertificate(
            SafeSslContextHandle ctx,
            SafeX509Handle certPtr
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxUsePrivateKey")]
        partial internal static int SslCtxUsePrivateKey(
            SafeSslContextHandle ctx,
            SafeEvpPKeyHandle keyPtr
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxCheckPrivateKey")]
        partial internal static int SslCtxCheckPrivateKey(SafeSslContextHandle ctx);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxSetQuietShutdown")]
        partial internal static void SslCtxSetQuietShutdown(SafeSslContextHandle ctx);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SslCtxSetCiphers")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool SslCtxSetCiphers(
            SafeSslContextHandle ctx,
            byte* cipherList,
            byte* cipherSuites
        );

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_SslCtxSetEncryptionPolicy"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SetEncryptionPolicy(
            SafeSslContextHandle ctx,
            EncryptionPolicy policy
        );

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_SslCtxSetDefaultOcspCallback"
        )]
        partial internal static void SslCtxSetDefaultOcspCallback(SafeSslContextHandle ctx);
    }
}
