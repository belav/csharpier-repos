// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class User32
    {
        [LibraryImport(Libraries.User32, EntryPoint = "LoadStringW", SetLastError = true)]
        partial internal static unsafe int LoadString(
            IntPtr hInstance,
            uint uID,
            char* lpBuffer,
            int cchBufferMax
        );
    }
}
