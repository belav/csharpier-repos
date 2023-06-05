// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial internal static class CapiHelper
    {
        // Return PROV_RSA_AES, in case any compat case pops up.
        internal const int DefaultRsaProviderType = 24;
    }
}
