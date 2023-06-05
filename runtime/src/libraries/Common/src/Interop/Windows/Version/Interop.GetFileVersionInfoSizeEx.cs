// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Version
    {
        [LibraryImport(
            Libraries.Version,
            EntryPoint = "GetFileVersionInfoSizeExW",
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static uint GetFileVersionInfoSizeEx(
            uint dwFlags,
            string lpwstrFilename,
            out uint lpdwHandle
        );
    }
}
