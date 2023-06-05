// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Net
{
    // From Schannel.h
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct SecPkgContext_CipherInfo
    {
        private const int SZ_ALG_MAX_SIZE = 64;

        private readonly int dwVersion;
        private readonly int dwProtocol;
        public readonly int dwCipherSuite;
        private readonly int dwBaseCipherSuite;
        fixed private char szCipherSuite[SZ_ALG_MAX_SIZE];
        fixed private char szCipher[SZ_ALG_MAX_SIZE];
        private readonly int dwCipherLen;
        private readonly int dwCipherBlockLen; // in bytes
        fixed private char szHash[SZ_ALG_MAX_SIZE];
        private readonly int dwHashLen;
        fixed private char szExchange[SZ_ALG_MAX_SIZE];
        private readonly int dwMinExchangeLen;
        private readonly int dwMaxExchangeLen;
        fixed private char szCertificate[SZ_ALG_MAX_SIZE];
        private readonly int dwKeyType;
    }
}
