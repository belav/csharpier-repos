// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const int CTRL_C_EVENT = 0;
        internal const int CTRL_BREAK_EVENT = 1;
        internal const int CTRL_CLOSE_EVENT = 2;
        internal const int CTRL_LOGOFF_EVENT = 5;
        internal const int CTRL_SHUTDOWN_EVENT = 6;

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool SetConsoleCtrlHandler(
            delegate* unmanaged<int, BOOL> handler,
            [MarshalAs(UnmanagedType.Bool)] bool Add
        );
    }
}
