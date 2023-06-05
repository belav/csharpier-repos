// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class IpHlpApi
    {
        [LibraryImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        partial internal static uint if_nametoindex([MarshalAs(UnmanagedType.LPStr)] string name);
    }
}
