// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System.Security.Cryptography
{
    partial public class DSA : AsymmetricAlgorithm
    {
        private static DSA CreateCore()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
