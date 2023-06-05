// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(Libraries.GlobalizationNative, EntryPoint = "GlobalizationNative_LoadICU")]
        partial internal static int LoadICU();

        internal static void InitICUFunctions(
            IntPtr icuuc,
            IntPtr icuin,
            ReadOnlySpan<char> version,
            ReadOnlySpan<char> suffix
        )
        {
            Debug.Assert(icuuc != IntPtr.Zero);
            Debug.Assert(icuin != IntPtr.Zero);

            InitICUFunctions(
                icuuc,
                icuin,
                version.ToString(),
                suffix.Length > 0 ? suffix.ToString() : null
            );
        }

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_InitICUFunctions",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static void InitICUFunctions(
            IntPtr icuuc,
            IntPtr icuin,
            string version,
            string? suffix
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetICUVersion"
        )]
        partial internal static int GetICUVersion();
    }
}
