// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use RemoveDirectory.
        /// </summary>
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "RemoveDirectoryW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool RemoveDirectoryPrivate(string path);

        internal static bool RemoveDirectory(string path)
        {
            path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
            return RemoveDirectoryPrivate(path);
        }
    }
}
