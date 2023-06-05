// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CMSG_RECIPIENT_ENCRYPTED_KEY_INFO
        {
            internal CERT_ID RecipientId;
            internal DATA_BLOB EncryptedKey;
            internal FILETIME Date;
            internal CRYPT_ATTRIBUTE_TYPE_VALUE* pOtherAttr;
        }
    }
}
