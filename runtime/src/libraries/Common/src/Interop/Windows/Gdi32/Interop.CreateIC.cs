// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(Libraries.Gdi32, StringMarshalling = StringMarshalling.Utf16)]
        partial public static IntPtr CreateICW(
            string pszDriver,
            string pszDevice,
            string? pszPort,
            IntPtr pdm
        );
    }
}
