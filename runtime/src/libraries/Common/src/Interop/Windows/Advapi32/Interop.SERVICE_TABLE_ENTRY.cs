// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Advapi32
    {
        public delegate void ServiceMainCallback(int argCount, IntPtr argPointer);

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_TABLE_ENTRY
        {
            public IntPtr name;
            public IntPtr callback;
        }
    }
}
