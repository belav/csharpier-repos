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
        partial internal static unsafe NTSTATUS BCryptGenerateSymmetricKey(
            SafeBCryptAlgorithmHandle hAlgorithm,
            out SafeBCryptKeyHandle phKey,
            IntPtr pbKeyObject,
            int cbKeyObject,
            byte* pbSecret,
            int cbSecret,
            uint dwFlags
        );

        [LibraryImport(Libraries.BCrypt)]
        partial internal static unsafe NTSTATUS BCryptGenerateSymmetricKey(
            nuint hAlgorithm,
            out SafeBCryptKeyHandle phKey,
            IntPtr pbKeyObject,
            int cbKeyObject,
            byte* pbSecret,
            int cbSecret,
            uint dwFlags
        );
    }
}
