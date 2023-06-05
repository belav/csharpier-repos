// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class OleAut32
    {
        // only using this to clear existing error info with null
        [LibraryImport(Interop.Libraries.OleAut32)]
        // TLS values are preserved between threads, need to check that we use this API to clear the error state only.
        partial
        // TLS values are preserved between threads, need to check that we use this API to clear the error state only.
        internal static void SetErrorInfo(int dwReserved, IntPtr pIErrorInfo);
    }
}
