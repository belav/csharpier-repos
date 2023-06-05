partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Crypt32
    {
        internal enum CryptMsgType : int
        {
            CMSG_DATA = 1,
            CMSG_SIGNED = 2,
            CMSG_ENVELOPED = 3,
            CMSG_SIGNED_AND_ENVELOPED = 4,
            CMSG_HASHED = 5,
            CMSG_ENCRYPTED = 6,
        }
    }
}
