// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class OleAut32
    {
        [LibraryImport(Libraries.OleAut32, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static IntPtr SysAllocStringLen(IntPtr src, uint len);

        [LibraryImport(Libraries.OleAut32, StringMarshalling = StringMarshalling.Utf16)]
        partial internal static IntPtr SysAllocStringLen(string src, uint len);
    }
}
