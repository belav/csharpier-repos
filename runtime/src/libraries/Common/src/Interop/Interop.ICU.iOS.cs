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
            EntryPoint = "GlobalizationNative_LoadICUData",
            StringMarshalling = StringMarshalling.Utf8
        )]
        partial internal static int LoadICUData(string path);
    }
}
