// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        internal const uint CRYPTPROTECTMEMORY_BLOCK_SIZE = 16;
        internal const uint CRYPTPROTECTMEMORY_SAME_PROCESS = 0;

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptProtectMemory(
            SafeBuffer pData,
            uint cbData,
            uint dwFlags
        );

        [LibraryImport(Libraries.Crypt32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool CryptUnprotectMemory(
            SafeBuffer pData,
            uint cbData,
            uint dwFlags
        );
    }
}
