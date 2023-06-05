// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPKeyCtxCreate")]
        partial internal static SafeEvpPKeyCtxHandle EvpPKeyCtxCreate(
            SafeEvpPKeyHandle pkey,
            SafeEvpPKeyHandle peerkey,
            out uint secretLength
        );

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_EvpPKeyDeriveSecretAgreement"
        )]
        partial private static int EvpPKeyDeriveSecretAgreement(
            ref byte secret,
            uint secretLength,
            SafeEvpPKeyCtxHandle ctx
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPKeyCtxDestroy")]
        partial internal static void EvpPKeyCtxDestroy(IntPtr ctx);

        internal static void EvpPKeyDeriveSecretAgreement(
            SafeEvpPKeyCtxHandle ctx,
            Span<byte> destination
        )
        {
            Debug.Assert(ctx != null);
            Debug.Assert(!ctx.IsInvalid);

            int ret = EvpPKeyDeriveSecretAgreement(
                ref MemoryMarshal.GetReference(destination),
                (uint)destination.Length,
                ctx
            );

            if (ret != 1)
            {
                throw CreateOpenSslCryptographicException();
            }
        }
    }
}
