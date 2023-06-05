// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class SspiCli
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct TOKEN_SOURCE
        {
            fixed internal byte SourceName[TOKEN_SOURCE_LENGTH];
            internal LUID SourceIdentifier;

            internal const int TOKEN_SOURCE_LENGTH = 8;
        }
    }
}
