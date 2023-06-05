// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class NCrypt
    {
        [LibraryImport(Libraries.NCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static unsafe ErrorCode NCryptSignHash(
            SafeNCryptKeyHandle hKey,
            void* pPaddingInfo,
            ReadOnlySpan<byte> pbHashValue,
            int cbHashValue,
            Span<byte> pbSignature,
            int cbSignature,
            out int pcbResult,
            AsymmetricPaddingMode dwFlags
        );

        [LibraryImport(Libraries.NCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static unsafe ErrorCode NCryptVerifySignature(
            SafeNCryptKeyHandle hKey,
            void* pPaddingInfo,
            ReadOnlySpan<byte> pbHashValue,
            int cbHashValue,
            ReadOnlySpan<byte> pbSignature,
            int cbSignature,
            AsymmetricPaddingMode dwFlags
        );
    }
}
