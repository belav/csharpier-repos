// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static NTSTATUS BCryptOpenAlgorithmProvider(
            out SafeBCryptAlgorithmHandle phAlgorithm,
            string pszAlgId,
            string? pszImplementation,
            BCryptOpenAlgorithmProviderFlags dwFlags
        );

        internal static SafeBCryptAlgorithmHandle BCryptOpenAlgorithmProvider(
            string pszAlgId,
            string? pszImplementation = null,
            BCryptOpenAlgorithmProviderFlags dwFlags = 0
        )
        {
            NTSTATUS status = BCryptOpenAlgorithmProvider(
                out SafeBCryptAlgorithmHandle hAlgorithm,
                pszAlgId,
                pszImplementation,
                dwFlags
            );

            if (status != NTSTATUS.STATUS_SUCCESS)
            {
                hAlgorithm.Dispose();
                throw CreateCryptographicException(status);
            }

            return hAlgorithm;
        }

        [Flags]
        internal enum BCryptOpenAlgorithmProviderFlags : int
        {
            None = 0x00000000,
            BCRYPT_ALG_HANDLE_HMAC_FLAG = 0x00000008,
        }
    }
}
