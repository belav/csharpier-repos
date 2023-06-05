// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_Disconnect")]
        partial internal static Error Disconnect(IntPtr socket);
    }
}
