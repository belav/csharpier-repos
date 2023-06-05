// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class IpHlpApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IP_ADDR_STRING
        {
            public IP_ADDR_STRING* Next;
            fixed public byte IpAddress[16];
            fixed public byte IpMask[16];
            public uint Context;
        }
    }
}
