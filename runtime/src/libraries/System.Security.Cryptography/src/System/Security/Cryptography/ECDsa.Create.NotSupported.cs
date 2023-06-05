// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public class ECDsa : ECAlgorithm
    {
        partial public static new ECDsa Create()
        {
            throw new PlatformNotSupportedException();
        }

        partial public static ECDsa Create(ECCurve curve)
        {
            throw new PlatformNotSupportedException();
        }

        partial public static ECDsa Create(ECParameters parameters)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
