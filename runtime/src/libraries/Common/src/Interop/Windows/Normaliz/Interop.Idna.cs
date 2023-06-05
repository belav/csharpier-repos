// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Normaliz
    {
        //
        //  Idn APIs
        //

        [LibraryImport(
            "Normaliz.dll",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int IdnToAscii(
            uint dwFlags,
            char* lpUnicodeCharStr,
            int cchUnicodeChar,
            char* lpASCIICharStr,
            int cchASCIIChar
        );

        [LibraryImport(
            "Normaliz.dll",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int IdnToUnicode(
            uint dwFlags,
            char* lpASCIICharStr,
            int cchASCIIChar,
            char* lpUnicodeCharStr,
            int cchUnicodeChar
        );

        internal const int IDN_ALLOW_UNASSIGNED = 0x1;
        internal const int IDN_USE_STD3_ASCII_RULES = 0x2;
    }
}
