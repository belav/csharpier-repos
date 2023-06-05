// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        [LibraryImport(Libraries.WinMM)]
        partial internal static unsafe int mmioAscend(IntPtr hMIO, MMCKINFO* lpck, int flags);
    }
}
