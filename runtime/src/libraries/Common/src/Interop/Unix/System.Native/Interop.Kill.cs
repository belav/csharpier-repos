// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal enum Signals : int
        {
            None = 0,
            SIGKILL = 9,
            SIGSTOP = 19
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_Kill",
            SetLastError = true
        )]
        partial internal static int Kill(int pid, Signals signal);
    }
}
