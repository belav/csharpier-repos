partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Crypt32
    {
        internal enum CMsgKeyAgreeOriginatorChoice : int
        {
            CMSG_KEY_AGREE_ORIGINATOR_CERT = 1,
            CMSG_KEY_AGREE_ORIGINATOR_PUBLIC_KEY = 2,
        }
    }
}
