// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.IO.Compression;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Brotli
    {
        [LibraryImport(Libraries.CompressionNative)]
        partial internal static SafeBrotliDecoderHandle BrotliDecoderCreateInstance(
            IntPtr allocFunc,
            IntPtr freeFunc,
            IntPtr opaque
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static unsafe int BrotliDecoderDecompressStream(
            SafeBrotliDecoderHandle state,
            ref nuint availableIn,
            byte** nextIn,
            ref nuint availableOut,
            byte** nextOut,
            out nuint totalOut
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static unsafe BOOL BrotliDecoderDecompress(
            nuint availableInput,
            byte* inBytes,
            nuint* availableOutput,
            byte* outBytes
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static void BrotliDecoderDestroyInstance(IntPtr state);

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static BOOL BrotliDecoderIsFinished(SafeBrotliDecoderHandle state);

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static SafeBrotliEncoderHandle BrotliEncoderCreateInstance(
            IntPtr allocFunc,
            IntPtr freeFunc,
            IntPtr opaque
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static BOOL BrotliEncoderSetParameter(
            SafeBrotliEncoderHandle state,
            BrotliEncoderParameter parameter,
            uint value
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static unsafe BOOL BrotliEncoderCompressStream(
            SafeBrotliEncoderHandle state,
            BrotliEncoderOperation op,
            ref nuint availableIn,
            byte** nextIn,
            ref nuint availableOut,
            byte** nextOut,
            out nuint totalOut
        );

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static BOOL BrotliEncoderHasMoreOutput(SafeBrotliEncoderHandle state);

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static void BrotliEncoderDestroyInstance(IntPtr state);

        [LibraryImport(Libraries.CompressionNative)]
        partial internal static unsafe BOOL BrotliEncoderCompress(
            int quality,
            int window,
            int v,
            nuint availableInput,
            byte* inBytes,
            nuint* availableOutput,
            byte* outBytes
        );
    }
}
