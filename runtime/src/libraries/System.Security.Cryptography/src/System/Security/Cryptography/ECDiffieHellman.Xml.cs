// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography
{
    partial public abstract class ECDiffieHellman : ECAlgorithm
    {
        public override void FromXmlString(string xmlString)
        {
            throw new NotImplementedException(SR.Cryptography_ECXmlSerializationFormatRequired);
        }

        public override string ToXmlString(bool includePrivateParameters)
        {
            throw new NotImplementedException(SR.Cryptography_ECXmlSerializationFormatRequired);
        }
    }
}
