// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO.Compression;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class ZLib
    {
        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_DeflateInit2_")]
        partial internal static unsafe ZLibNative.ErrorCode DeflateInit2_(
            ZLibNative.ZStream* stream,
            ZLibNative.CompressionLevel level,
            ZLibNative.CompressionMethod method,
            int windowBits,
            int memLevel,
            ZLibNative.CompressionStrategy strategy
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_Deflate")]
        partial internal static unsafe ZLibNative.ErrorCode Deflate(
            ZLibNative.ZStream* stream,
            ZLibNative.FlushCode flush
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_DeflateReset")]
        partial internal static unsafe ZLibNative.ErrorCode DeflateReset(
            ZLibNative.ZStream* stream
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_DeflateEnd")]
        partial internal static unsafe ZLibNative.ErrorCode DeflateEnd(ZLibNative.ZStream* stream);

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_InflateInit2_")]
        partial internal static unsafe ZLibNative.ErrorCode InflateInit2_(
            ZLibNative.ZStream* stream,
            int windowBits
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_Inflate")]
        partial internal static unsafe ZLibNative.ErrorCode Inflate(
            ZLibNative.ZStream* stream,
            ZLibNative.FlushCode flush
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_InflateReset")]
        partial internal static unsafe ZLibNative.ErrorCode InflateReset(
            ZLibNative.ZStream* stream
        );

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_InflateEnd")]
        partial internal static unsafe ZLibNative.ErrorCode InflateEnd(ZLibNative.ZStream* stream);

        [LibraryImport(Libraries.CompressionNative, EntryPoint = "CompressionNative_Crc32")]
        partial internal static unsafe uint crc32(uint crc, byte* buffer, int len);
    }
}
