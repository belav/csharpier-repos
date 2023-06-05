// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        [LibraryImport(Libraries.BCrypt)]
        partial private static unsafe NTSTATUS BCryptFinalizeKeyPair(
            SafeBCryptKeyHandle hKey,
            uint dwFlags
        );

        internal static void BCryptFinalizeKeyPair(SafeBCryptKeyHandle key)
        {
            NTSTATUS status = BCryptFinalizeKeyPair(key, 0);

            if (status != NTSTATUS.STATUS_SUCCESS)
            {
                throw CreateCryptographicException(status);
            }
        }
    }
}
