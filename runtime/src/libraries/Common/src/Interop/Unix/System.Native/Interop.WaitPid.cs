// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        /// <summary>
        /// Reaps a terminated child.
        /// </summary>
        /// <returns>
        /// 1) when a child is reaped, its process id is returned
        /// 2) if pid is not a child or there are no unwaited-for children, -1 is returned (errno=ECHILD)
        /// 3) if the child has not yet terminated, 0 is returned
        /// 4) on error, -1 is returned.
        /// </returns>
        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_WaitPidExitedNoHang",
            SetLastError = true
        )]
        partial internal static int WaitPidExitedNoHang(int pid, out int exitCode);
    }
}
