// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal struct TimeSpec
        {
            internal long TvSec;
            internal long TvNsec;
        }

        /// <summary>
        /// Sets the last access and last modified time of a file
        /// </summary>
        /// <param name="path">The path to the item to get time values for</param>
        /// <param name="times">The output time values of the item</param>
        /// <returns>
        /// Returns 0 on success; otherwise, returns -1
        /// </returns>
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_UTimensat",
            StringMarshalling = StringMarshalling.Utf8,
            SetLastError = true
        )]
        partial internal static unsafe int UTimensat(string path, TimeSpec* times);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_FUTimens",
            SetLastError = true
        )]
        partial internal static unsafe int FUTimens(SafeHandle fd, TimeSpec* times);
    }
}
