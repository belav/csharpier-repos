// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        internal struct TOKEN_PRIVILEGE
        {
            public uint PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges /*[ANYSIZE_ARRAY]*/
            ;
        }
    }
}
