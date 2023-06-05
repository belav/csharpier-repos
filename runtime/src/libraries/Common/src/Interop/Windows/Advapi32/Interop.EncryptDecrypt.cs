// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use EncryptFile.
        /// </summary>
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "EncryptFileW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool EncryptFilePrivate(string lpFileName);

        internal static bool EncryptFile(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
            return EncryptFilePrivate(path);
        }

        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use DecryptFile.
        /// </summary>
        [LibraryImport(
            Libraries.Advapi32,
            EntryPoint = "DecryptFileW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool DecryptFileFilePrivate(string lpFileName, int dwReserved);

        internal static bool DecryptFile(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
            return DecryptFileFilePrivate(path, 0);
        }
    }
}
