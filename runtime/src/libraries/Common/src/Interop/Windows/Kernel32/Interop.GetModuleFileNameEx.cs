// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [LibraryImport(
            Libraries.Kernel32,
            EntryPoint = "K32GetModuleFileNameExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16
        )]
        partial internal static int GetModuleFileNameEx(
            SafeProcessHandle processHandle,
            IntPtr moduleHandle,
            [Out] char[] baseName,
            int size
        );
    }
}
