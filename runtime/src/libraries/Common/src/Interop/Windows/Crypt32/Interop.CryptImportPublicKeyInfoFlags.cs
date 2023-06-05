// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [Flags]
        internal enum CryptImportPublicKeyInfoFlags
        {
            NONE = 0,
            CRYPT_OID_INFO_PUBKEY_ENCRYPT_KEY_FLAG = 0x40000000,
        }
    }
}
