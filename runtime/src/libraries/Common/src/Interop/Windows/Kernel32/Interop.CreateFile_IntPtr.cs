// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use CreateFile.
        /// </summary>
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "CreateFileW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial private static unsafe IntPtr CreateFilePrivate_IntPtr(
            string lpFileName,
            int dwDesiredAccess,
            FileShare dwShareMode,
            SECURITY_ATTRIBUTES* lpSecurityAttributes,
            FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        internal static unsafe IntPtr CreateFile_IntPtr(
            string lpFileName,
            int dwDesiredAccess,
            FileShare dwShareMode,
            FileMode dwCreationDisposition,
            int dwFlagsAndAttributes
        )
        {
            lpFileName = PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
            return CreateFilePrivate_IntPtr(
                lpFileName,
                dwDesiredAccess,
                dwShareMode,
                null,
                dwCreationDisposition,
                dwFlagsAndAttributes,
                IntPtr.Zero
            );
        }
    }
}
