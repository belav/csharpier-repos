using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography;

namespace Microsoft.AspNetCore.DataProtection.Managed;

internal static class HashAlgorithmExtensions
{
    public static int GetDigestSizeInBytes(this HashAlgorithm hashAlgorithm)
    {
        var hashSizeInBits = hashAlgorithm.HashSize;
        CryptoUtil.Assert(
            hashSizeInBits >= 0 && hashSizeInBits % 8 == 0,
            "hashSizeInBits >= 0 && hashSizeInBits % 8 == 0"
        );
        return hashSizeInBits / 8;
    }
}
