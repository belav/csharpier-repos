// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public abstract class ECDiffieHellman : ECAlgorithm
    {
        partial public static new ECDiffieHellman Create()
        {
            return new ECDiffieHellmanWrapper(new ECDiffieHellmanOpenSsl());
        }

        partial public static ECDiffieHellman Create(ECCurve curve)
        {
            return new ECDiffieHellmanWrapper(new ECDiffieHellmanOpenSsl(curve));
        }

        partial public static ECDiffieHellman Create(ECParameters parameters)
        {
            ECDiffieHellman ecdh = new ECDiffieHellmanOpenSsl();

            try
            {
                ecdh.ImportParameters(parameters);
                return new ECDiffieHellmanWrapper(ecdh);
            }
            catch
            {
                ecdh.Dispose();
                throw;
            }
        }
    }
}
