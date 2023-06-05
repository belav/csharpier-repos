// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use SetFileAttributes.
        /// </summary>
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "SetFileAttributesW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool SetFileAttributesPrivate(string name, int attr);

        internal static bool SetFileAttributes(string name, int attr)
        {
            name = PathInternal.EnsureExtendedPrefixIfNeeded(name);
            return SetFileAttributesPrivate(name, attr);
        }
    }
}
