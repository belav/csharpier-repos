// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class Crypt32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_PUBLIC_KEY_INFO
        {
            internal CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            internal CRYPT_BIT_BLOB PublicKey;
        }
    }
}
