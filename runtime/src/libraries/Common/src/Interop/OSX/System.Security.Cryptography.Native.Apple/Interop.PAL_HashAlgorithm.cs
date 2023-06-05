partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class AppleCrypto
    {
        internal enum PAL_HashAlgorithm
        {
            Unknown = 0,
            Md5,
            Sha1,
            Sha256,
            Sha384,
            Sha512,
        }
    }
}
