// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.Sockets;

partial internal static class Interop
{
    partial internal static class Sys
    {
        internal unsafe struct MessageHeader
        {
            public byte* SocketAddress;
            public IOVector* IOVectors;
            public byte* ControlBuffer;
            public int SocketAddressLen;
            public int IOVectorCount;
            public int ControlBufferLen;
            public SocketFlags Flags;
        }
    }
}
