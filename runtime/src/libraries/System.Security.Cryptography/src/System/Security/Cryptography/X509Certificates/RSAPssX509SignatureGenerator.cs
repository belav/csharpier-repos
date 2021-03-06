// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates
{
    internal sealed class RSAPssX509SignatureGenerator : X509SignatureGenerator
    {
        private readonly RSA _key;
        private readonly RSASignaturePadding _padding;

        internal RSAPssX509SignatureGenerator(RSA key, RSASignaturePadding padding)
        {
            Debug.Assert(key != null);
            Debug.Assert(padding != null);
            Debug.Assert(padding.Mode == RSASignaturePaddingMode.Pss);

            // Currently we don't accept options in PSS mode, but we could, so store the padding here.
            _key = key;
            _padding = padding;
        }

        public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
        {
            // If we ever support options in PSS (like MGF-2, if such an MGF is ever invented)
            // Or, more reasonably, supporting a custom value for the salt size.
            if (_padding != RSASignaturePadding.Pss)
            {
                throw new CryptographicException(SR.Cryptography_InvalidPaddingMode);
            }

            int cbSalt;
            string digestOid;

            if (hashAlgorithm == HashAlgorithmName.SHA256)
            {
                cbSalt = SHA256.HashSizeInBytes;
                digestOid = Oids.Sha256;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA384)
            {
                cbSalt = SHA384.HashSizeInBytes;
                digestOid = Oids.Sha384;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA512)
            {
                cbSalt = SHA512.HashSizeInBytes;
                digestOid = Oids.Sha512;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hashAlgorithm),
                    hashAlgorithm,
                    SR.Format(SR.Cryptography_UnknownHashAlgorithm, hashAlgorithm.Name));
            }

            // RFC 5754 says that the NULL for SHA2 (256/384/512) MUST be omitted
            // (https://tools.ietf.org/html/rfc5754#section-2) (and that you MUST
            // be able to read it even if someone wrote it down)
            //
            // Since we
            //  * don't support SHA-1 in this class
            //  * only support MGF-1
            //  * don't support the MGF PRF being different than hashAlgorithm
            //  * use saltLength==hashLength
            //  * don't allow custom trailer
            // we don't have to worry about any of the DEFAULTs. (specify, specify, specify, omit).

            PssParamsAsn parameters = new PssParamsAsn
            {
                HashAlgorithm = new AlgorithmIdentifierAsn { Algorithm = digestOid },
                MaskGenAlgorithm = new AlgorithmIdentifierAsn { Algorithm = Oids.Mgf1 },
                SaltLength = cbSalt,
                TrailerField = 1,
            };

            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);

            using (writer.PushSequence())
            {
                writer.WriteObjectIdentifierForCrypto(digestOid);
            }

            parameters.MaskGenAlgorithm.Parameters = writer.Encode();
            writer.Reset();

            parameters.Encode(writer);

            AlgorithmIdentifierAsn identifier = new AlgorithmIdentifierAsn
            {
                Algorithm = Oids.RsaPss,
                Parameters = writer.Encode(),
            };

            writer.Reset();
            identifier.Encode(writer);
            return writer.Encode();
        }

        public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
        {
            return _key.SignData(data, hashAlgorithm, _padding);
        }

        protected override PublicKey BuildPublicKey()
        {
            // RFC 4055 (https://tools.ietf.org/html/rfc4055) recommends using a different
            // key format for 'PSS keys'.  RFC 5756 (https://tools.ietf.org/html/rfc5756) says
            // that almost no one did that, and that it goes against the general guidance of
            // SubjectPublicKeyInfo, so RSA keys should use the existing form always and the
            // PSS-specific key algorithm ID is deprecated (as a key ID).
            return RSAPkcs1X509SignatureGenerator.BuildPublicKey(_key);
        }
    }
}
