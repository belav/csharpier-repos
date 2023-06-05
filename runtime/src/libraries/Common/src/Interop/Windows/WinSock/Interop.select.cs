// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Winsock
    {
        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static unsafe int select(
            int ignoredParameter,
            IntPtr* readfds,
            IntPtr* writefds,
            IntPtr* exceptfds,
            ref TimeValue timeout
        );

        [LibraryImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        partial internal static unsafe int select(
            int ignoredParameter,
            IntPtr* readfds,
            IntPtr* writefds,
            IntPtr* exceptfds,
            IntPtr nullTimeout
        );
    }
}
