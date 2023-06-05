// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreate2")]
        partial internal static SafeEvpCipherCtxHandle EvpCipherCreate(
            IntPtr cipher,
            ref byte key,
            int keyLength,
            ref byte iv,
            int enc
        );

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreatePartial")]
        partial internal static SafeEvpCipherCtxHandle EvpCipherCreatePartial(IntPtr cipher);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetKeyAndIV")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherSetKeyAndIV(
            SafeEvpCipherCtxHandle ctx,
            ref byte key,
            ref byte iv,
            EvpCipherDirection direction
        );

        internal static void EvpCipherSetKeyAndIV(
            SafeEvpCipherCtxHandle ctx,
            ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> iv,
            EvpCipherDirection direction
        )
        {
            if (
                !EvpCipherSetKeyAndIV(
                    ctx,
                    ref MemoryMarshal.GetReference(key),
                    ref MemoryMarshal.GetReference(iv),
                    direction
                )
            )
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_EvpCipherSetGcmNonceLength"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool CryptoNative_EvpCipherSetGcmNonceLength(
            SafeEvpCipherCtxHandle ctx,
            int nonceLength
        );

        internal static void EvpCipherSetGcmNonceLength(SafeEvpCipherCtxHandle ctx, int nonceLength)
        {
            if (!CryptoNative_EvpCipherSetGcmNonceLength(ctx, nonceLength))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_EvpCipherSetCcmNonceLength"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool CryptoNative_EvpCipherSetCcmNonceLength(
            SafeEvpCipherCtxHandle ctx,
            int nonceLength
        );

        internal static void EvpCipherSetCcmNonceLength(SafeEvpCipherCtxHandle ctx, int nonceLength)
        {
            if (!CryptoNative_EvpCipherSetCcmNonceLength(ctx, nonceLength))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherDestroy")]
        partial internal static void EvpCipherDestroy(IntPtr ctx);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherReset")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static unsafe bool EvpCipherReset(
            SafeEvpCipherCtxHandle ctx,
            byte* pIv,
            int cIv
        );

        internal static unsafe bool EvpCipherReset(
            SafeEvpCipherCtxHandle ctx,
            ReadOnlySpan<byte> iv
        )
        {
            fixed (byte* pIv = &MemoryMarshal.GetReference(iv))
            {
                return EvpCipherReset(ctx, pIv, iv.Length);
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCtxSetPadding")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool EvpCipherCtxSetPadding(SafeEvpCipherCtxHandle x, int padding);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherUpdate")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            ref byte output,
            out int outl,
            ref byte input,
            int inl
        );

        internal static bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            Span<byte> output,
            out int bytesWritten,
            ReadOnlySpan<byte> input
        )
        {
            return EvpCipherUpdate(
                ctx,
                ref MemoryMarshal.GetReference(output),
                out bytesWritten,
                ref MemoryMarshal.GetReference(input),
                input.Length
            );
        }

        internal static void EvpCipherSetInputLength(SafeEvpCipherCtxHandle ctx, int inputLength)
        {
            if (
                !EvpCipherUpdate(
                    ctx,
                    ref Unsafe.NullRef<byte>(),
                    out _,
                    ref Unsafe.NullRef<byte>(),
                    inputLength
                )
            )
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherFinalEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            ref byte outm,
            out int outl
        );

        internal static bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            Span<byte> output,
            out int bytesWritten
        )
        {
            return EvpCipherFinalEx(ctx, ref MemoryMarshal.GetReference(output), out bytesWritten);
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetGcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherGetGcmTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherGetGcmTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            if (!EvpCipherGetGcmTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetAeadTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherGetAeadTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherGetAeadTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            if (!EvpCipherGetAeadTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetGcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherSetGcmTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherSetGcmTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            if (!EvpCipherSetGcmTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetAeadTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherSetAeadTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherSetAeadTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            if (!EvpCipherSetAeadTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherGetCcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherGetCcmTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherGetCcmTag(SafeEvpCipherCtxHandle ctx, Span<byte> tag)
        {
            if (!EvpCipherGetCcmTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetCcmTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EvpCipherSetCcmTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength
        );

        internal static void EvpCipherSetCcmTag(SafeEvpCipherCtxHandle ctx, ReadOnlySpan<byte> tag)
        {
            if (!EvpCipherSetCcmTag(ctx, ref MemoryMarshal.GetReference(tag), tag.Length))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        internal static void EvpCipherSetCcmTagLength(SafeEvpCipherCtxHandle ctx, int tagLength)
        {
            if (!EvpCipherSetCcmTag(ctx, ref Unsafe.NullRef<byte>(), tagLength))
            {
                throw CreateOpenSslCryptographicException();
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ecb")]
        partial internal static IntPtr EvpAes128Ecb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cbc")]
        partial internal static IntPtr EvpAes128Cbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Gcm")]
        partial internal static IntPtr EvpAes128Gcm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cfb8")]
        partial internal static IntPtr EvpAes128Cfb8();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cfb128")]
        partial internal static IntPtr EvpAes128Cfb128();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ccm")]
        partial internal static IntPtr EvpAes128Ccm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ecb")]
        partial internal static IntPtr EvpAes192Ecb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cbc")]
        partial internal static IntPtr EvpAes192Cbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Gcm")]
        partial internal static IntPtr EvpAes192Gcm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cfb8")]
        partial internal static IntPtr EvpAes192Cfb8();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cfb128")]
        partial internal static IntPtr EvpAes192Cfb128();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ccm")]
        partial internal static IntPtr EvpAes192Ccm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ecb")]
        partial internal static IntPtr EvpAes256Ecb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cbc")]
        partial internal static IntPtr EvpAes256Cbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Gcm")]
        partial internal static IntPtr EvpAes256Gcm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cfb128")]
        partial internal static IntPtr EvpAes256Cfb128();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cfb8")]
        partial internal static IntPtr EvpAes256Cfb8();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ccm")]
        partial internal static IntPtr EvpAes256Ccm();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesCbc")]
        partial internal static IntPtr EvpDesCbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesEcb")]
        partial internal static IntPtr EvpDesEcb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesCfb8")]
        partial internal static IntPtr EvpDesCfb8();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cbc")]
        partial internal static IntPtr EvpDes3Cbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Ecb")]
        partial internal static IntPtr EvpDes3Ecb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cfb8")]
        partial internal static IntPtr EvpDes3Cfb8();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cfb64")]
        partial internal static IntPtr EvpDes3Cfb64();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Cbc")]
        partial internal static IntPtr EvpRC2Cbc();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Ecb")]
        partial internal static IntPtr EvpRC2Ecb();

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpChaCha20Poly1305")]
        partial internal static IntPtr EvpChaCha20Poly1305();

        internal enum EvpCipherDirection : int
        {
            NoChange = -1,
            Decrypt = 0,
            Encrypt = 1,
        }
    }
}
