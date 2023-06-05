// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

partial internal static class Interop
{
    partial internal static class NCrypt
    {
        [Flags]
        internal enum SecretAgreementFlags
        {
            None = 0x00000000,
            UseSecretAsHmacKey = 0x00000001 // KDF_USE_SECRET_AS_HMAC_KEY_FLAG
        }

        /// <summary>
        ///     Generate a secret agreement for generating shared key material
        /// </summary>
        [LibraryImport(Interop.Libraries.NCrypt)]
        partial private static ErrorCode NCryptSecretAgreement(
            SafeNCryptKeyHandle hPrivKey,
            SafeNCryptKeyHandle hPubKey,
            out SafeNCryptSecretHandle phSecret,
            int dwFlags
        );

        /// <summary>
        /// Generate a secret agreement value for between two parties
        /// </summary>
        internal static SafeNCryptSecretHandle DeriveSecretAgreement(
            SafeNCryptKeyHandle privateKey,
            SafeNCryptKeyHandle otherPartyPublicKey
        )
        {
            ErrorCode error = NCryptSecretAgreement(
                privateKey,
                otherPartyPublicKey,
                out SafeNCryptSecretHandle secretAgreement,
                0
            );

            if (error != ErrorCode.ERROR_SUCCESS)
            {
                secretAgreement.Dispose();
                throw error.ToCryptographicException();
            }

            return secretAgreement;
        }
    }
}
