// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CMSG_KEY_TRANS_RECIPIENT_INFO
        {
            internal int dwVersion;
            internal CERT_ID RecipientId;
            internal CRYPT_ALGORITHM_IDENTIFIER KeyEncryptionAlgorithm;
            internal DATA_BLOB EncryptedKey;
        }
    }
}
