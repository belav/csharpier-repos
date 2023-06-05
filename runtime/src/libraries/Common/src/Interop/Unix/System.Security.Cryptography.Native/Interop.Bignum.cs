// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BigNumDestroy")]
        partial internal static void BigNumDestroy(IntPtr a);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BigNumFromBinary")]
        partial private static unsafe SafeBignumHandle BigNumFromBinary(
            ReadOnlySpan<byte> bigEndianValue,
            int len
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BigNumToBinary")]
        partial private static unsafe int BigNumToBinary(SafeBignumHandle a, byte* to);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetBigNumBytes")]
        partial private static int GetBigNumBytes(SafeBignumHandle a);

        internal static SafeBignumHandle CreateBignum(ReadOnlySpan<byte> bigEndianValue)
        {
            SafeBignumHandle ret = BigNumFromBinary(bigEndianValue, bigEndianValue.Length);
            if (ret.IsInvalid)
            {
                Exception e = CreateOpenSslCryptographicException();
                ret.Dispose();
                throw e;
            }

            return ret;
        }

        internal static byte[]? ExtractBignum(IntPtr bignum, int targetSize)
        {
            // Given that the only reference held to bignum is an IntPtr, create an unowned SafeHandle
            // to ensure that we don't destroy the key after extraction.
            using (SafeBignumHandle handle = new SafeBignumHandle(bignum, ownsHandle: false))
            {
                return ExtractBignum(handle, targetSize);
            }
        }

        internal static unsafe byte[]? ExtractBignum(SafeBignumHandle? bignum, int targetSize)
        {
            if (bignum == null || bignum.IsInvalid)
            {
                return null;
            }

            int compactSize = GetBigNumBytes(bignum);

            if (targetSize < compactSize)
            {
                targetSize = compactSize;
            }

            // OpenSSL BIGNUM values do not record leading zeroes.
            // Windows Crypt32 does.
            //
            // Since RSACryptoServiceProvider already checks that RSAParameters.DP.Length is
            // exactly half of RSAParameters.Modulus.Length, we need to left-pad (big-endian)
            // the array with zeroes.
            int offset = targetSize - compactSize;

            byte[] buf = new byte[targetSize];

            fixed (byte* to = buf)
            {
                byte* start = to + offset;
                BigNumToBinary(bignum, start);
            }

            return buf;
        }
    }
}
