// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacCreate")]
        partial internal static SafeHmacCtxHandle HmacCreate(ref byte key, int keyLen, IntPtr md);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacDestroy")]
        partial internal static void HmacDestroy(IntPtr ctx);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacReset")]
        partial internal static int HmacReset(SafeHmacCtxHandle ctx);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacUpdate")]
        partial internal static int HmacUpdate(
            SafeHmacCtxHandle ctx,
            ReadOnlySpan<byte> data,
            int len
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacFinal")]
        partial internal static int HmacFinal(SafeHmacCtxHandle ctx, ref byte data, ref int len);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacCurrent")]
        partial internal static int HmacCurrent(SafeHmacCtxHandle ctx, ref byte data, ref int len);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_HmacOneShot")]
        partial private static unsafe int HmacOneShot(
            IntPtr type,
            byte* key,
            int keySize,
            byte* source,
            int sourceSize,
            byte* md,
            int* mdSize
        );

        internal static unsafe int HmacOneShot(
            IntPtr type,
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> source,
            Span<byte> destination
        )
        {
            int size = destination.Length;
            const int Success = 1;

            fixed (byte* pKey = key)
            fixed (byte* pSource = source)
            fixed (byte* pDestination = destination)
            {
                int result = HmacOneShot(
                    type,
                    pKey,
                    key.Length,
                    pSource,
                    source.Length,
                    pDestination,
                    &size
                );

                if (result != Success)
                {
                    Debug.Assert(result == 0);
                    throw CreateOpenSslCryptographicException();
                }
            }

            return size;
        }
    }
}
