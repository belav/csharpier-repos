// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal enum SysConfName
        {
            _SC_CLK_TCK = 1,
            _SC_PAGESIZE = 2
        }

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_SysConf",
            SetLastError = true
        )]
        partial internal static long SysConf(SysConfName name);
    }
}
