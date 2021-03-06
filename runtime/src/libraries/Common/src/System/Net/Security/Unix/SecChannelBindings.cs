// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Net.Security
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecChannelBindings
    {
        internal int InitiatorAddrType;
        internal int InitiatorLength;
        internal int InitiatorOffset;
        internal int AcceptorAddrType;
        internal int AcceptorLength;
        internal int AcceptorOffset;
        internal int ApplicationDataLength;
        internal int ApplicationDataOffset;
    }
}
