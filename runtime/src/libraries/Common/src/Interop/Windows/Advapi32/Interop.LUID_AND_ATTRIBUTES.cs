// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        [StructLayout(LayoutKind.Sequential)]
        partial internal struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }
    }
}
