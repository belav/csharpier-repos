// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public class RSA : AsymmetricAlgorithm
    {
        partial public static new RSA Create()
        {
            return new RSABCrypt();
        }
    }
}
