// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial
    // mkfifo: https://man7.org/linux/man-pages/man3/mkfifo.3.html
    internal static class Sys
    {
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_MkFifo",
            StringMarshalling = StringMarshalling.Utf8,
            SetLastError = true
        )]
        partial internal static int MkFifo(string pathName, uint mode);
    }
}
