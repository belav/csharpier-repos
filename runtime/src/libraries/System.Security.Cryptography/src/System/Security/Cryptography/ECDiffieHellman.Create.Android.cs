// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public abstract class ECDiffieHellman : ECAlgorithm
    {
        partial public static new ECDiffieHellman Create()
        {
            return new ECDiffieHellmanImplementation.ECDiffieHellmanAndroid();
        }

        partial public static ECDiffieHellman Create(ECCurve curve)
        {
            return new ECDiffieHellmanImplementation.ECDiffieHellmanAndroid(curve);
        }

        partial public static ECDiffieHellman Create(ECParameters parameters)
        {
            ECDiffieHellman ecdh = new ECDiffieHellmanImplementation.ECDiffieHellmanAndroid();

            try
            {
                ecdh.ImportParameters(parameters);
                return ecdh;
            }
            catch
            {
                ecdh.Dispose();
                throw;
            }
        }
    }
}
