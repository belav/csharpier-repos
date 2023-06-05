// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Gdi32
    {
        [LibraryImport(
            Libraries.Gdi32,
            EntryPoint = "AddFontResourceExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int AddFontResourceEx(string lpszFilename, int fl, IntPtr pdv);

        internal static int AddFontFile(string fileName)
        {
            return AddFontResourceEx(
                fileName, /*FR_PRIVATE*/
                0x10,
                IntPtr.Zero
            );
        }
    }
}
