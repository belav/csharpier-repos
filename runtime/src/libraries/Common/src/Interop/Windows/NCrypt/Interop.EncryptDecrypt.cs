// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class NCrypt
    {
        [LibraryImport(Interop.Libraries.NCrypt)]
        partial internal static unsafe ErrorCode NCryptEncrypt(
            SafeNCryptKeyHandle hKey,
            ReadOnlySpan<byte> pbInput,
            int cbInput,
            void* pPaddingInfo,
            Span<byte> pbOutput,
            int cbOutput,
            out int pcbResult,
            AsymmetricPaddingMode dwFlags
        );

        [LibraryImport(Interop.Libraries.NCrypt)]
        partial internal static unsafe ErrorCode NCryptDecrypt(
            SafeNCryptKeyHandle hKey,
            ReadOnlySpan<byte> pbInput,
            int cbInput,
            void* pPaddingInfo,
            Span<byte> pbOutput,
            int cbOutput,
            out int pcbResult,
            AsymmetricPaddingMode dwFlags
        );
    }
}
