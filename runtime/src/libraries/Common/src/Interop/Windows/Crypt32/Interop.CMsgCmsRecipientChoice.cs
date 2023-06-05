partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Crypt32
    {
        internal enum CMsgCmsRecipientChoice : int
        {
            CMSG_KEY_TRANS_RECIPIENT = 1,
            CMSG_KEY_AGREE_RECIPIENT = 2,
            CMSG_MAIL_LIST_RECIPIENT = 3,
        }
    }
}
