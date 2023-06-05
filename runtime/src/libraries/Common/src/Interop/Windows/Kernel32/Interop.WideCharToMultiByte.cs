// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe int WideCharToMultiByte(
            uint CodePage,
            uint dwFlags,
            char* lpWideCharStr,
            int cchWideChar,
            byte* lpMultiByteStr,
            int cbMultiByte,
            byte* lpDefaultChar,
            BOOL* lpUsedDefaultChar
        );

        internal const uint CP_ACP = 0;
        internal const uint WC_NO_BEST_FIT_CHARS = 0x00000400;
    }
}
