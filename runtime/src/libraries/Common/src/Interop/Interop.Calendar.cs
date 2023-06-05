// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetCalendars",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int GetCalendars(
            string localeName,
            CalendarId[] calendars,
            int calendarsCapacity
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetCalendarInfo",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe ResultCode GetCalendarInfo(
            string localeName,
            CalendarId calendarId,
            CalendarDataType calendarDataType,
            char* result,
            int resultCapacity
        );

        internal static unsafe bool EnumCalendarInfo(
            delegate* unmanaged<char*, IntPtr, void> callback,
            string localeName,
            CalendarId calendarId,
            CalendarDataType calendarDataType,
            IntPtr context
        )
        {
            return EnumCalendarInfo(
                (IntPtr)callback,
                localeName,
                calendarId,
                calendarDataType,
                context
            );
        }

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_EnumCalendarInfo",
            StringMarshalling = StringMarshalling.Utf16
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        // We skip the following DllImport because of 'Parsing function pointer types in signatures is not supported.' for some targeted
        // platforms (for example, WASM build).
        partial
        // We skip the following DllImport because of 'Parsing function pointer types in signatures is not supported.' for some targeted
        // platforms (for example, WASM build).
        private static unsafe bool EnumCalendarInfo(
            IntPtr callback,
            string localeName,
            CalendarId calendarId,
            CalendarDataType calendarDataType,
            IntPtr context
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetLatestJapaneseEra"
        )]
        partial internal static int GetLatestJapaneseEra();

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetJapaneseEraStartDate"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool GetJapaneseEraStartDate(
            int era,
            out int startYear,
            out int startMonth,
            out int startDay
        );
    }
}
