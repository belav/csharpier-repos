// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal struct LingerOption
        {
            public int OnOff; // Non-zero to enable linger
            public int Seconds; // Number of seconds to linger for
        }

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetLingerOption")]
        partial internal static unsafe Error GetLingerOption(
            SafeHandle socket,
            LingerOption* option
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetLingerOption")]
        partial internal static unsafe Error SetLingerOption(
            SafeHandle socket,
            LingerOption* option
        );

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetLingerOption")]
        partial internal static unsafe Error SetLingerOption(IntPtr socket, LingerOption* option);
    }
}
