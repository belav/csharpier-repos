// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CONDITION_VARIABLE
        {
            private IntPtr Ptr;
        }

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe void InitializeConditionVariable(
            CONDITION_VARIABLE* ConditionVariable
        );

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe void WakeConditionVariable(
            CONDITION_VARIABLE* ConditionVariable
        );

        [LibraryImport(Libraries.Kernel32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        partial internal static unsafe bool SleepConditionVariableCS(
            CONDITION_VARIABLE* ConditionVariable,
            CRITICAL_SECTION* CriticalSection,
            int dwMilliseconds
        );
    }
}
