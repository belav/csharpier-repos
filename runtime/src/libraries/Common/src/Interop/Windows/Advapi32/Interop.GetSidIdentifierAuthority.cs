// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SID_IDENTIFIER_AUTHORITY
        {
            public byte b1;
            public byte b2;
            public byte b3;
            public byte b4;
            public byte b5;
            public byte b6;
        }

        [LibraryImport(Libraries.Advapi32)]
        partial internal static IntPtr GetSidIdentifierAuthority(IntPtr sid);

        [LibraryImport(Interop.Libraries.Advapi32)]
        partial internal static IntPtr GetSidSubAuthority(IntPtr sid, int index);

        [LibraryImport(Interop.Libraries.Advapi32)]
        partial internal static IntPtr GetSidSubAuthorityCount(IntPtr sid);
    }
}
