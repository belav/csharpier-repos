// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

partial internal static class Interop
{
    partial internal static class BCrypt
    {
        internal const string BCRYPT_CHAIN_MODE_CBC = "ChainingModeCBC";
        internal const string BCRYPT_CHAIN_MODE_ECB = "ChainingModeECB";
        internal const string BCRYPT_CHAIN_MODE_CFB = "ChainingModeCFB";
    }
}
