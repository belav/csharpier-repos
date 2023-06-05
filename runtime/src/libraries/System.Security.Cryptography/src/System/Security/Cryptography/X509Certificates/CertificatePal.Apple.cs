// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates
{
    partial internal static class CertificatePal
    {
        partial internal static ICertificatePal FromHandle(IntPtr handle)
        {
            return AppleCertificatePal.FromHandle(handle, true)!;
        }

        partial internal static ICertificatePal FromOtherCert(X509Certificate copyFrom)
        {
            return AppleCertificatePal.FromOtherCert(copyFrom);
        }

        partial internal static ICertificatePal FromBlob(
            ReadOnlySpan<byte> rawData,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            return AppleCertificatePal.FromBlob(rawData, password, keyStorageFlags);
        }

        partial internal static ICertificatePal FromFile(
            string fileName,
            SafePasswordHandle password,
            X509KeyStorageFlags keyStorageFlags
        )
        {
            return AppleCertificatePal.FromFile(fileName, password, keyStorageFlags);
        }
    }
}
