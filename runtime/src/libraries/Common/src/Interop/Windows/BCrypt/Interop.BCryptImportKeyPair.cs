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
        partial private static unsafe NTSTATUS BCryptImportKeyPair(
            SafeBCryptAlgorithmHandle hAlgorithm,
            IntPtr hImportKey,
            string pszBlobType,
            out SafeBCryptKeyHandle phKey,
            byte* pbInput,
            int cbInput,
            uint dwFlags
        );

        internal static unsafe SafeBCryptKeyHandle BCryptImportKeyPair(
            SafeBCryptAlgorithmHandle algorithm,
            string blobType,
            ReadOnlySpan<byte> keyBlob
        )
        {
            NTSTATUS status;
            SafeBCryptKeyHandle key;

            fixed (byte* pBlob = keyBlob)
            {
                status = BCryptImportKeyPair(
                    algorithm,
                    IntPtr.Zero,
                    blobType,
                    out key,
                    pBlob,
                    keyBlob.Length,
                    0
                );
            }

            if (status != NTSTATUS.STATUS_SUCCESS)
            {
                key.Dispose();
                throw CreateCryptographicException(status);
            }

            return key;
        }
    }
}
