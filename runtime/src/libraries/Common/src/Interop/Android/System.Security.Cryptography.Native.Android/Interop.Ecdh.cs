// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

partial internal static class Interop
{
    partial internal static class AndroidCrypto
    {
        internal static bool EcdhDeriveKey(
            SafeEcKeyHandle ourKey,
            SafeEcKeyHandle peerKey,
            Span<byte> buffer,
            out int usedBuffer
        ) =>
            EcdhDeriveKey(
                ourKey,
                peerKey,
                ref MemoryMarshal.GetReference(buffer),
                buffer.Length,
                out usedBuffer
            );

        [LibraryImport(
            Libraries.AndroidCryptoNative,
            EntryPoint = "AndroidCryptoNative_EcdhDeriveKey"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EcdhDeriveKey(
            SafeEcKeyHandle ourKey,
            SafeEcKeyHandle peerKey,
            ref byte buffer,
            int bufferLength,
            out int usedBuffer
        );
    }
}
