// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct ProcessCpuInformation
        {
            internal ulong lastRecordedCurrentTime;
            internal ulong lastRecordedKernelTime;
            internal ulong lastRecordedUserTime;
        }

        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetCpuUtilization")]
        partial internal static double GetCpuUtilization(ref ProcessCpuInformation previousCpuInfo);
    }
}
