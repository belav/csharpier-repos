// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class WinMM
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct MMCKINFO
        {
            internal int ckID;
            internal int cksize;
            internal int fccType;
            internal int dwDataOffset;
            internal int dwFlags;
        }
    }
}
