// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt)]
        partial private static unsafe NTSTATUS BCryptGenerateKeyPair(
            SafeBCryptAlgorithmHandle hAlgorithm,
            out SafeBCryptKeyHandle phKey,
            int dwLength,
            uint dwFlags
        );

        internal static SafeBCryptKeyHandle BCryptGenerateKeyPair(
            SafeBCryptAlgorithmHandle hAlgorithm,
            int keyLength
        )
        {
            NTSTATUS status = BCryptGenerateKeyPair(
                hAlgorithm,
                out SafeBCryptKeyHandle hKey,
                keyLength,
                0
            );

            if (status != NTSTATUS.STATUS_SUCCESS)
            {
                hKey.Dispose();
                throw CreateCryptographicException(status);
            }

            return hKey;
        }
    }
}
