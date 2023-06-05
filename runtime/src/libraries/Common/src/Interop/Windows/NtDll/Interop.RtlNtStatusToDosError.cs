// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class NtDll
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms680600(v=vs.85).aspx
        [LibraryImport(Libraries.NtDll)]
        partial public static uint RtlNtStatusToDosError(int Status);
    }
}
