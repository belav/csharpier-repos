// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        internal const int WAIT_FAILED = unchecked((int)0xFFFFFFFF);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static uint WaitForMultipleObjectsEx(
            uint nCount,
            IntPtr lpHandles,
            BOOL bWaitAll,
            uint dwMilliseconds,
            BOOL bAlertable
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static uint SignalObjectAndWait(
            IntPtr hObjectToSignal,
            IntPtr hObjectToWaitOn,
            uint dwMilliseconds,
            BOOL bAlertable
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static void Sleep(uint milliseconds);

        internal const uint CREATE_SUSPENDED = 0x00000004;
        internal const uint STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000;

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe SafeWaitHandle CreateThread(
            IntPtr lpThreadAttributes,
            IntPtr dwStackSize,
            delegate* unmanaged<IntPtr, uint> lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            out uint lpThreadId
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static uint ResumeThread(SafeWaitHandle hThread);

        [LibraryImport(Libraries.Kernel32)]
        partial internal static IntPtr GetCurrentThread();

        internal const int DUPLICATE_SAME_ACCESS = 2;

        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool DuplicateHandle(
            IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle,
            IntPtr hTargetProcessHandle,
            out SafeWaitHandle lpTargetHandle,
            uint dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            uint dwOptions
        );

        internal enum ThreadPriority : int
        {
            Idle = -15,
            Lowest = -2,
            BelowNormal = -1,
            Normal = 0,
            AboveNormal = 1,
            Highest = 2,
            TimeCritical = 15,

            ErrorReturn = 0x7FFFFFFF
        }

        [LibraryImport(Libraries.Kernel32)]
        partial internal static ThreadPriority GetThreadPriority(SafeWaitHandle hThread);

        [LibraryImport(Libraries.Kernel32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static bool SetThreadPriority(SafeWaitHandle hThread, int nPriority);
    }
}
