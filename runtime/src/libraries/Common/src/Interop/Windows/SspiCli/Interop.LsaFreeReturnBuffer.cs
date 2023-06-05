// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class SspiCli
    {
        [LibraryImport(Interop.Libraries.SspiCli, SetLastError = true)]
        partial internal static int LsaFreeReturnBuffer(IntPtr handle);
    }
}
