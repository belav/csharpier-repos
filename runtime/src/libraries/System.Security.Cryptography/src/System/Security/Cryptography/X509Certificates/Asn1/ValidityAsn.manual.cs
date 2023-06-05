// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    partial internal struct ValidityAsn
    {
        public ValidityAsn(DateTimeOffset notBefore, DateTimeOffset notAfter)
        {
            NotBefore = new TimeAsn(notBefore);
            NotAfter = new TimeAsn(notAfter);
        }
    }
}
