// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Globalization
    {
        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_GetTimeZoneDisplayName",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe ResultCode GetTimeZoneDisplayName(
            string localeName,
            string timeZoneId,
            TimeZoneDisplayNameType type,
            char* result,
            int resultLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_WindowsIdToIanaId",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int WindowsIdToIanaId(
            string windowsId,
            IntPtr region,
            char* ianaId,
            int ianaIdLength
        );

        [LibraryImport(
            Libraries.GlobalizationNative,
            EntryPoint = "GlobalizationNative_IanaIdToWindowsId",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static unsafe int IanaIdToWindowsId(
            string ianaId,
            char* windowsId,
            int windowsIdLength
        );
    }
}
