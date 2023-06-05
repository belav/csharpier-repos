// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal unsafe class Sys
    {
        [LibraryImport(
            Interop.Libraries.SystemNative,
            StringMarshalling = StringMarshalling.Utf8,
            EntryPoint = "SystemNative_GetEnv"
        )]
        partial internal static unsafe IntPtr GetEnv(string name);
    }
}
