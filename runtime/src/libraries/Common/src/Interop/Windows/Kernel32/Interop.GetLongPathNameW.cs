// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetFullPath/PathHelper.
        /// </summary>
        [LibraryImport(
            Libraries.Kernel32,
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static uint GetLongPathNameW(
            ref char lpszShortPath,
            ref char lpszLongPath,
            uint cchBuffer
        );
    }
}
