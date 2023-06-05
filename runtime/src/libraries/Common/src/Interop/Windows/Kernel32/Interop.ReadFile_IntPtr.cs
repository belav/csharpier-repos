// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        partial internal static unsafe int ReadFile(
            IntPtr handle,
            byte* bytes,
            int numBytesToRead,
            out int numBytesRead,
            IntPtr mustBeZero
        );
    }
}
