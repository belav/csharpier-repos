// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Sys
    {
        partial internal static class Fcntl
        {
            internal static readonly bool CanGetSetPipeSz = (FcntlCanGetSetPipeSz() != 0);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlGetPipeSz",
                SetLastError = true
            )]
            partial internal static int GetPipeSz(SafePipeHandle fd);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlSetPipeSz",
                SetLastError = true
            )]
            partial internal static int SetPipeSz(SafePipeHandle fd, int size);

            [LibraryImport(
                Libraries.SystemNative,
                EntryPoint = "SystemNative_FcntlCanGetSetPipeSz"
            )]
            [SuppressGCTransition]
            partial private static int FcntlCanGetSetPipeSz();
        }
    }
}
