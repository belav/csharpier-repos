// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocaleName",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetLocaleName(
            string localeName,
            char* value,
            int valueLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocaleInfoString",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetLocaleInfoString(
            string localeName,
            uint localeStringData,
            char* value,
            int valueLength,
            string? uiLocaleName = null
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetDefaultLocaleName",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetDefaultLocaleName(char* value, int valueLength);

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_IsPredefinedLocale",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool IsPredefinedLocale(string localeName);

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocaleTimeFormat",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool GetLocaleTimeFormat(
            string localeName,
            [MarshalAs(UnmanagedType.Bool)] bool shortFormat,
            char* value,
            int valueLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocaleInfoInt",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool GetLocaleInfoInt(
            string localeName,
            uint localeNumberData,
            ref int value
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocaleInfoGroupingSizes",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool GetLocaleInfoGroupingSizes(
            string localeName,
            uint localeGroupingData,
            ref int primaryGroupSize,
            ref int secondaryGroupSize
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLocales",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int GetLocales([Out] char[]? value, int valueLength);
    }
}
