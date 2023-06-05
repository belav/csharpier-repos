// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Cryptography.X509Certificates
{
    partial internal static class ChainPal
    {
#pragma warning disable IDE0060
        partial
#pragma warning disable IDE0060
        internal static IChainPal FromHandle(IntPtr chainContext)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static bool ReleaseSafeX509ChainHandle(IntPtr handle)
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }

        partial internal static IChainPal? BuildChain(
            bool useMachineContext,
            ICertificatePal cert,
            X509Certificate2Collection? extraStore,
            OidCollection? applicationPolicy,
            OidCollection? certificatePolicy,
            X509RevocationMode revocationMode,
            X509RevocationFlag revocationFlag,
            X509Certificate2Collection? customTrustStore,
            X509ChainTrustMode trustMode,
            DateTime verificationTime,
            TimeSpan timeout,
            bool disableAia
        )
        {
            throw new PlatformNotSupportedException(
                SR.SystemSecurityCryptographyX509Certificates_PlatformNotSupported
            );
        }
#pragma warning restore IDE0060
    }
}
