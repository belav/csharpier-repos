// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public class ECDiffieHellman : ECAlgorithm
    {
        partial public static new ECDiffieHellman Create()
        {
            throw new PlatformNotSupportedException();
        }

        partial public static ECDiffieHellman Create(ECCurve curve)
        {
            throw new PlatformNotSupportedException();
        }

        partial public static ECDiffieHellman Create(ECParameters parameters)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
