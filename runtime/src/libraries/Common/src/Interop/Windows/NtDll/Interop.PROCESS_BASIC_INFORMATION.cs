// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class NtDll
    {
        // From PROCESSINFOCLASS
        // Use for NtQueryInformationProcess
        internal const int ProcessBasicInformation = 0;

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            public uint ExitStatus;
            public IntPtr PebBaseAddress;
            public UIntPtr AffinityMask;
            public int BasePriority;
            public UIntPtr UniqueProcessId;
            public UIntPtr InheritedFromUniqueProcessId;
        }
    }
}
