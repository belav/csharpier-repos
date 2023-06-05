// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CPINFO
        {
            internal int MaxCharSize;

            fixed internal byte DefaultChar[
                2 /* MAX_DEFAULTCHAR */
            ];
            fixed internal byte LeadByte[
                12 /* MAX_LEADBYTES */
            ];
        }

        [LibraryImport(Libraries.Kernel32)]
        partial internal static unsafe Interop.BOOL GetCPInfo(uint codePage, CPINFO* lpCpInfo);
    }
}
