// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        internal static SafeBCryptHashHandle BCryptDuplicateHash(SafeBCryptHashHandle hHash)
        {
            SafeBCryptHashHandle newHash;
            NTSTATUS status = BCryptDuplicateHash(hHash, out newHash, IntPtr.Zero, 0, 0);

            if (status != NTSTATUS.STATUS_SUCCESS)
            {
                newHash.Dispose();
                throw CreateCryptographicException(status);
            }

            return newHash;
        }

        [LibraryImport(Libraries.BCrypt)]
        partial private static NTSTATUS BCryptDuplicateHash(
            SafeBCryptHashHandle hHash,
            out SafeBCryptHashHandle phNewHash,
            IntPtr pbHashObject,
            int cbHashObject,
            int dwFlags
        );
    }
}
