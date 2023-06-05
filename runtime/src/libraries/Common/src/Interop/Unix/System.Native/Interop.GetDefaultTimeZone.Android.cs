// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(
            Interop.Libraries.SystemNative,
            EntryPoint = "SystemNative_GetDefaultTimeZone",
            StringMarshalling = StringMarshalling.Utf8,
            SetLastError = true
        )]
        partial internal static string? GetDefaultTimeZone();
    }
}
