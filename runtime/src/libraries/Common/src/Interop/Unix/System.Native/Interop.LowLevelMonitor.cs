// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_LowLevelMonitor_Create")]
        partial internal static IntPtr LowLevelMonitor_Create();

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_LowLevelMonitor_Destroy")]
        partial internal static void LowLevelMonitor_Destroy(IntPtr monitor);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_LowLevelMonitor_Acquire")]
        partial internal static void LowLevelMonitor_Acquire(IntPtr monitor);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_LowLevelMonitor_Release")]
        partial internal static void LowLevelMonitor_Release(IntPtr monitor);

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_LowLevelMonitor_Wait")]
        partial internal static void LowLevelMonitor_Wait(IntPtr monitor);

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_LowLevelMonitor_TimedWait"
        )]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool LowLevelMonitor_TimedWait(
            IntPtr monitor,
            int timeoutMilliseconds
        );

        [LibraryImport(
            Libraries.SystemNative,
            EntryPoint = "SystemNative_LowLevelMonitor_Signal_Release"
        )]
        partial internal static void LowLevelMonitor_Signal_Release(IntPtr monitor);
    }
}
