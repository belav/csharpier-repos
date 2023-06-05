// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        /// <summary>
        /// Deletes the specified empty directory.
        /// </summary>
        /// <param name="path">The path of the directory to delete</param>
        /// <returns>
        /// Returns 0 on success; otherwise, returns -1
        /// </returns>
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_RmDir",
            StringMarshalling = StringMarshalling.Utf8,
            SetLastError = true
        )]
        partial internal static int RmDir(string path);
    }
}
