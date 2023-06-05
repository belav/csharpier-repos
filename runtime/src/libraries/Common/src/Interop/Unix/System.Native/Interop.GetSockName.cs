// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetSockName")]
        partial internal static unsafe Error GetSockName(
            SafeHandle socket,
            byte* socketAddress,
            int* socketAddressLen
        );
    }
}
