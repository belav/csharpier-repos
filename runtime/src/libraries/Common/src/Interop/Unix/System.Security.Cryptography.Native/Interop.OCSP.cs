// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OcspRequestDestroy")]
        partial internal static void OcspRequestDestroy(IntPtr ocspReq);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetOcspRequestDerSize")]
        partial internal static int GetOcspRequestDerSize(SafeOcspRequestHandle req);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EncodeOcspRequest")]
        partial internal static int EncodeOcspRequest(SafeOcspRequestHandle req, byte[] buf);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_X509BuildOcspRequest")]
        partial internal static SafeOcspRequestHandle X509BuildOcspRequest(
            IntPtr subject,
            IntPtr issuer
        );

        [LibraryImport(Libraries.CryptoNative)]
        partial private static unsafe int CryptoNative_X509DecodeOcspToExpiration(
            byte* buf,
            int len,
            SafeOcspRequestHandle req,
            IntPtr subject,
            IntPtr issuer,
            ref long expiration
        );

        internal static unsafe bool X509DecodeOcspToExpiration(
            ReadOnlySpan<byte> buf,
            SafeOcspRequestHandle request,
            IntPtr x509Subject,
            IntPtr x509Issuer,
            out DateTimeOffset expiration
        )
        {
            long timeT = 0;
            int ret;

            fixed (byte* pBuf = buf)
            {
                ret = CryptoNative_X509DecodeOcspToExpiration(
                    pBuf,
                    buf.Length,
                    request,
                    x509Subject,
                    x509Issuer,
                    ref timeT
                );
            }

            if (ret == 1)
            {
                if (timeT != 0)
                {
                    expiration = DateTimeOffset.FromUnixTimeSeconds(timeT);
                }
                else
                {
                    // Something went wrong during the determination of when the response
                    // should not be used any longer.
                    // Half an hour sounds fair?
                    expiration = DateTimeOffset.UtcNow.AddMinutes(30);
                }

                return true;
            }

            Debug.Assert(ret == 0, $"Unexpected response from X509DecodeOcspToExpiration: {ret}");
            expiration = DateTimeOffset.MinValue;
            return false;
        }

        [LibraryImport(Libraries.CryptoNative)]
        partial private static SafeOcspResponseHandle CryptoNative_DecodeOcspResponse(
            ref byte buf,
            int len
        );

        internal static SafeOcspResponseHandle DecodeOcspResponse(ReadOnlySpan<byte> buf)
        {
            return CryptoNative_DecodeOcspResponse(ref MemoryMarshal.GetReference(buf), buf.Length);
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OcspResponseDestroy")]
        partial internal static void OcspResponseDestroy(IntPtr ocspReq);
    }
}

namespace System.Security.Cryptography.X509Certificates
{
    internal sealed class SafeOcspRequestHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeOcspRequestHandle()
            : base(true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.OcspRequestDestroy(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }

    internal sealed class SafeOcspResponseHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeOcspResponseHandle()
            : base(true) { }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.OcspResponseDestroy(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }
}
