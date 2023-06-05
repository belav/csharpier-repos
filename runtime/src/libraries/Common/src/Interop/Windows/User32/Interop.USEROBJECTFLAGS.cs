// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class User32
    {
        internal struct USEROBJECTFLAGS
        {
            public int fInherit;
            public int fReserved;
            public int dwFlags;
        }
    }
}
