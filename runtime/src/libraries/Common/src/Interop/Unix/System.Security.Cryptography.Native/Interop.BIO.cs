// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Crypto
    {
        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CreateMemoryBio")]
        partial internal static SafeBioHandle CreateMemoryBio();

        [LibraryImport(
            Libraries.CryptoNative,
            EntryPoint = "CryptoNative_BioNewFile",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static SafeBioHandle BioNewFile(string filename, string mode);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioDestroy")]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool BioDestroy(IntPtr a);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioGets")]
        partial private static unsafe int BioGets(SafeBioHandle b, byte* buf, int size);

        internal static unsafe int BioGets(SafeBioHandle b, Span<byte> buf)
        {
            fixed (byte* pBuf = buf)
            {
                return BioGets(b, pBuf, buf.Length);
            }
        }

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioRead")]
        partial internal static int BioRead(SafeBioHandle b, byte[] data, int len);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioWrite")]
        partial internal static int BioWrite(SafeBioHandle b, byte[] data, int len);

        internal static int BioWrite(SafeBioHandle b, ReadOnlySpan<byte> data) =>
            BioWrite(b, ref MemoryMarshal.GetReference(data), data.Length);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioWrite")]
        partial private static int BioWrite(SafeBioHandle b, ref byte data, int len);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetMemoryBioSize")]
        partial internal static int GetMemoryBioSize(SafeBioHandle bio);

        [LibraryImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioCtrlPending")]
        partial internal static int BioCtrlPending(SafeBioHandle bio);
    }
}
