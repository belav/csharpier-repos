// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "ReplaceFileW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial private static bool ReplaceFilePrivate(
            string replacedFileName,
            string replacementFileName,
            string? backupFileName,
            int dwReplaceFlags,
            IntPtr lpExclude,
            IntPtr lpReserved
        );

        internal static bool ReplaceFile(
            string replacedFileName,
            string replacementFileName,
            string? backupFileName,
            int dwReplaceFlags,
            IntPtr lpExclude,
            IntPtr lpReserved
        )
        {
            replacedFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacedFileName);
            replacementFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacementFileName);
            backupFileName = PathInternal.EnsureExtendedPrefixIfNeeded(backupFileName);

            return ReplaceFilePrivate(
                replacedFileName,
                replacementFileName,
                backupFileName,
                dwReplaceFlags,
                lpExclude,
                lpReserved
            );
        }

        internal const int REPLACEFILE_IGNORE_MERGE_ERRORS = 0x2;
    }
}
