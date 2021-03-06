// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography
{
    internal static partial class CngPkcs8
    {
        // Windows 7, 8, and 8.1 don't support PBES2 export, so use
        // the 3DES-192 scheme from PKCS12-PBE whenever deferring to the system.
        //
        // Since we're going to immediately re-encrypt the value when using this,
        // and still have the password in memory while it's running,
        // just use one iteration in the KDF and cut down on the CPU time involved.
        private static readonly PbeParameters s_platformParameters =
            new PbeParameters(
                PbeEncryptionAlgorithm.TripleDes3KeyPkcs12,
                HashAlgorithmName.SHA1,
                1);

        internal static bool IsPlatformScheme(PbeParameters pbeParameters)
        {
            Debug.Assert(pbeParameters != null);

            return pbeParameters.EncryptionAlgorithm == s_platformParameters.EncryptionAlgorithm &&
                   pbeParameters.HashAlgorithm == s_platformParameters.HashAlgorithm;
        }

        internal static byte[] ExportEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<byte> passwordBytes,
            PbeParameters pbeParameters)
        {
            ArgumentNullException.ThrowIfNull(pbeParameters);

            PasswordBasedEncryption.ValidatePbeParameters(
                pbeParameters,
                ReadOnlySpan<char>.Empty,
                passwordBytes);

            if (passwordBytes.Length == 0)
            {
                // Switch to character-based, since that's the native input format.
                return key.ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char>.Empty, pbeParameters);
            }

            AsnWriter writer = RewriteEncryptedPkcs8PrivateKey(key, passwordBytes, pbeParameters);
            return writer.Encode();
        }

        internal static bool TryExportEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<byte> passwordBytes,
            PbeParameters pbeParameters,
            Span<byte> destination,
            out int bytesWritten)
        {
            if (passwordBytes.Length == 0)
            {
                // Switch to character-based, since that's the native input format.
                return key.TryExportEncryptedPkcs8PrivateKey(
                    ReadOnlySpan<char>.Empty,
                    pbeParameters,
                    destination,
                    out bytesWritten);
            }

            AsnWriter writer = RewriteEncryptedPkcs8PrivateKey(key, passwordBytes, pbeParameters);
            return writer.TryEncode(destination, out bytesWritten);
        }

        internal static byte[] ExportEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<char> password,
            PbeParameters pbeParameters)
        {
            AsnWriter writer = RewriteEncryptedPkcs8PrivateKey(key, password, pbeParameters);
            return writer.Encode();
        }

        internal static bool TryExportEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<char> password,
            PbeParameters pbeParameters,
            Span<byte> destination,
            out int bytesWritten)
        {
            AsnWriter writer = RewriteEncryptedPkcs8PrivateKey(key, password, pbeParameters);
            return writer.TryEncode(destination, out bytesWritten);
        }

        internal static Pkcs8Response ImportPkcs8PrivateKey(ReadOnlySpan<byte> source, out int bytesRead)
        {
            int len;

            try
            {
                AsnDecoder.ReadEncodedValue(
                    source,
                    AsnEncodingRules.BER,
                    out _,
                    out _,
                    out len);
            }
            catch (AsnContentException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }

            bytesRead = len;
            ReadOnlySpan<byte> pkcs8Source = source.Slice(0, len);

            try
            {
                return ImportPkcs8(pkcs8Source);
            }
            catch (CryptographicException)
            {
                AsnWriter? pkcs8ZeroPublicKey = RewritePkcs8ECPrivateKeyWithZeroPublicKey(pkcs8Source);

                if (pkcs8ZeroPublicKey == null)
                {
                    throw;
                }

                return ImportPkcs8(pkcs8ZeroPublicKey);
            }
            catch (AsnContentException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        private static Pkcs8Response ImportPkcs8(AsnWriter pkcs8Writer)
        {
            byte[] tmp = CryptoPool.Rent(pkcs8Writer.GetEncodedLength());

            if (!pkcs8Writer.TryEncode(tmp, out int written))
            {
                Debug.Fail("TryEncode failed with a pre-allocated buffer");
                throw new CryptographicException();
            }

            Pkcs8Response ret = ImportPkcs8(tmp.AsSpan(0, written));
            CryptoPool.Return(tmp, written);
            return ret;
        }

        internal static unsafe Pkcs8Response ImportEncryptedPkcs8PrivateKey(
            ReadOnlySpan<byte> passwordBytes,
            ReadOnlySpan<byte> source,
            out int bytesRead)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    try
                    {
                        // Since there's no bytes-based-password PKCS8 import in CNG, just do the decryption
                        // here and call the unencrypted PKCS8 import.
                        ArraySegment<byte> decrypted = KeyFormatHelper.DecryptPkcs8(
                            passwordBytes,
                            manager.Memory,
                            out bytesRead);

                        Span<byte> decryptedSpan = decrypted;

                        try
                        {
                            return ImportPkcs8(decryptedSpan);
                        }
                        catch (CryptographicException e)
                        {
                            AsnWriter? pkcs8ZeroPublicKey = RewritePkcs8ECPrivateKeyWithZeroPublicKey(decryptedSpan);

                            if (pkcs8ZeroPublicKey == null)
                            {
                                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
                            }

                            try
                            {
                                return ImportPkcs8(pkcs8ZeroPublicKey);
                            }
                            catch (CryptographicException)
                            {
                                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
                            }
                        }
                        finally
                        {
                            CryptoPool.Return(decrypted);
                        }
                    }
                    catch (AsnContentException e)
                    {
                        throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
                    }
                }
            }
        }

        internal static unsafe Pkcs8Response ImportEncryptedPkcs8PrivateKey(
           ReadOnlySpan<char> password,
           ReadOnlySpan<byte> source,
           out int bytesRead)
        {
            try
            {
                AsnDecoder.ReadEncodedValue(
                    source,
                    AsnEncodingRules.BER,
                    out _,
                    out _,
                    out int len);

                source = source.Slice(0, len);

                fixed (byte* ptr = &MemoryMarshal.GetReference(source))
                {
                    using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                    {
                        try
                        {
                            bytesRead = len;
                            return ImportPkcs8(source, password);
                        }
                        catch (CryptographicException)
                        {
                        }

                        ArraySegment<byte> decrypted = KeyFormatHelper.DecryptPkcs8(
                            password,
                            manager.Memory.Slice(0, len),
                            out int innerRead);

                        Span<byte> decryptedSpan = decrypted;

                        try
                        {
                            if (innerRead != len)
                            {
                                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding);
                            }

                            bytesRead = len;
                            return ImportPkcs8(decryptedSpan);
                        }
                        catch (CryptographicException e)
                        {
                            AsnWriter? pkcs8ZeroPublicKey = RewritePkcs8ECPrivateKeyWithZeroPublicKey(decryptedSpan);

                            if (pkcs8ZeroPublicKey == null)
                            {
                                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
                            }

                            try
                            {
                                bytesRead = len;
                                return ImportPkcs8(pkcs8ZeroPublicKey);
                            }
                            catch (CryptographicException)
                            {
                                throw new CryptographicException(SR.Cryptography_Pkcs8_EncryptedReadFailed, e);
                            }
                        }
                        finally
                        {
                            CryptoPool.Return(decrypted);
                        }
                    }
                }
            }
            catch (AsnContentException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        private static AsnWriter RewriteEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<byte> passwordBytes,
            PbeParameters pbeParameters)
        {
            Debug.Assert(pbeParameters != null);

            // For RSA:
            //  * 512-bit key needs ~400 bytes
            //  * 16384-bit key needs ~10k bytes.
            //  * KeySize (bits) should avoid re-rent.
            //
            // For DSA:
            //  * 512-bit key needs ~300 bytes.
            //  * 1024-bit key needs ~400 bytes.
            //  * 2048-bit key needs ~700 bytes.
            //  * KeySize (bits) should avoid re-rent.
            //
            // For ECC:
            //  * secp256r1 needs ~200 bytes (named) or ~450 (explicit)
            //  * secp384r1 needs ~250 bytes (named) or ~600 (explicit)
            //  * secp521r1 needs ~300 bytes (named) or ~730 (explicit)
            //  * KeySize (bits) should avoid re-rent for named, and probably
            //    gets one re-rent for explicit.
            byte[] rented = CryptoPool.Rent(key.KeySize);
            int rentWritten = 0;

            // If we use 6 bits from each byte, that's 22 * 6 = 132
            Span<char> randomString = stackalloc char[22];

            try
            {
                FillRandomAsciiString(randomString);

                while (!key.TryExportEncryptedPkcs8PrivateKey(
                    randomString,
                    s_platformParameters,
                    rented,
                    out rentWritten))
                {
                    int size = rented.Length;
                    byte[] current = rented;
                    rented = CryptoPool.Rent(checked(size * 2));
                    CryptoPool.Return(current, rentWritten);
                }

                return KeyFormatHelper.ReencryptPkcs8(
                    randomString,
                    rented.AsMemory(0, rentWritten),
                    passwordBytes,
                    pbeParameters);
            }
            finally
            {
                randomString.Clear();
                CryptoPool.Return(rented, rentWritten);
            }
        }

        private static AsnWriter RewriteEncryptedPkcs8PrivateKey(
            AsymmetricAlgorithm key,
            ReadOnlySpan<char> password,
            PbeParameters pbeParameters)
        {
            Debug.Assert(pbeParameters != null);

            byte[] rented = CryptoPool.Rent(key.KeySize);
            int rentWritten = 0;

            try
            {
                while (!key.TryExportEncryptedPkcs8PrivateKey(
                    password,
                    s_platformParameters,
                    rented,
                    out rentWritten))
                {
                    int size = rented.Length;
                    byte[] current = rented;
                    rented = CryptoPool.Rent(checked(size * 2));
                    CryptoPool.Return(current, rentWritten);
                }

                return KeyFormatHelper.ReencryptPkcs8(
                    password,
                    rented.AsMemory(0, rentWritten),
                    password,
                    pbeParameters);
            }
            finally
            {
                CryptoPool.Return(rented, rentWritten);
            }
        }

        // CNG cannot import a PrivateKeyInfo with the following criteria:
        // 1. Is a EC key with explicitly encoded parameters
        // 2. Is missing the PublicKey from ECPrivateKey.
        // CNG can import an explicit EC PrivateKeyInfo if the PublicKey
        // is present. CNG will also re-compute the public key from the
        // private key if they do not much. To help CNG be able to import
        // these keys, we re-write the PKCS8 to contain a zeroed PublicKey.
        //
        // If the PKCS8 key does not meet the above criteria, null is returned,
        // signaling the original exception should be thrown.
        private static unsafe AsnWriter? RewritePkcs8ECPrivateKeyWithZeroPublicKey(ReadOnlySpan<byte> source)
        {
            fixed (byte* ptr = &MemoryMarshal.GetReference(source))
            {
                using (MemoryManager<byte> manager = new PointerMemoryManager<byte>(ptr, source.Length))
                {
                    PrivateKeyInfoAsn privateKeyInfo = PrivateKeyInfoAsn.Decode(manager.Memory, AsnEncodingRules.BER);
                    AlgorithmIdentifierAsn privateAlgorithm = privateKeyInfo.PrivateKeyAlgorithm;

                    if (privateAlgorithm.Algorithm != Oids.EcPublicKey)
                    {
                        return null;
                    }

                    ECPrivateKey privateKey = ECPrivateKey.Decode(privateKeyInfo.PrivateKey, AsnEncodingRules.BER);
                    EccKeyFormatHelper.FromECPrivateKey(privateKey, privateAlgorithm, out ECParameters ecParameters);

                    fixed (byte* pD = ecParameters.D)
                    {
                        try
                        {
                            if (!ecParameters.Curve.IsExplicit || ecParameters.Q.X != null || ecParameters.Q.Y != null)
                            {
                                return null;
                            }

                            byte[] zero = new byte[ecParameters.D!.Length];
                            ecParameters.Q.Y = zero;
                            ecParameters.Q.X = zero;
                            return EccKeyFormatHelper.WritePkcs8PrivateKey(ecParameters, privateKeyInfo.Attributes);
                        }
                        finally
                        {
                            Array.Clear(ecParameters.D!);
                        }
                    }
                }
            }
        }

        private static void FillRandomAsciiString(Span<char> destination)
        {
            Debug.Assert(destination.Length < 128);
            Span<byte> randomKey = stackalloc byte[destination.Length];
            RandomNumberGenerator.Fill(randomKey);

            for (int i = 0; i < randomKey.Length; i++)
            {
                // 33 (!) up to 33 + 63 = 96 (`)
                destination[i] = (char)(33 + (randomKey[i] & 0b0011_1111));
            }
        }
    }
}
