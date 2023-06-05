// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class AppleCrypto
    {
        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainCreateDefaultPolicy"
        )]
        partial internal static SafeCreateHandle X509ChainCreateDefaultPolicy();

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainCreateRevocationPolicy"
        )]
        partial internal static SafeCreateHandle X509ChainCreateRevocationPolicy();

        [LibraryImport(Libraries.AppleCryptoNative)]
        partial internal static int AppleCryptoNative_X509ChainCreate(
            SafeCreateHandle certs,
            SafeCreateHandle policies,
            out SafeX509ChainHandle pTrustOut,
            out int pOSStatus
        );

        [LibraryImport(Libraries.AppleCryptoNative)]
        partial internal static int AppleCryptoNative_X509ChainEvaluate(
            SafeX509ChainHandle chain,
            SafeCFDateHandle cfEvaluationTime,
            [MarshalAs(UnmanagedType.Bool)] bool allowNetwork,
            out int pOSStatus
        );

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainGetChainSize"
        )]
        partial internal static long X509ChainGetChainSize(SafeX509ChainHandle chain);

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainGetCertificateAtIndex"
        )]
        partial internal static IntPtr X509ChainGetCertificateAtIndex(
            SafeX509ChainHandle chain,
            long index
        );

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainGetTrustResults"
        )]
        partial internal static SafeCreateHandle X509ChainGetTrustResults(
            SafeX509ChainHandle chain
        );

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainGetStatusAtIndex"
        )]
        partial internal static int X509ChainGetStatusAtIndex(
            SafeCreateHandle trustResults,
            long index,
            out int pdwStatus
        );

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_GetOSStatusForChainStatus"
        )]
        partial internal static int GetOSStatusForChainStatus(X509ChainStatusFlags flag);

        [LibraryImport(
            Libraries.AppleCryptoNative,
            EntryPoint = "AppleCryptoNative_X509ChainSetTrustAnchorCertificates"
        )]
        partial internal static int X509ChainSetTrustAnchorCertificates(
            SafeX509ChainHandle chain,
            SafeCreateHandle anchorCertificates
        );
    }
}
