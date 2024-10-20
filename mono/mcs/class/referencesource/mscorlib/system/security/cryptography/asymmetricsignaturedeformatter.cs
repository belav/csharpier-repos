// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
// <OWNER>Microsoft</OWNER>
//

//
// AsymmetricSignatureDeformatter.cs
//

namespace System.Security.Cryptography
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security;

    [System.Runtime.InteropServices.ComVisible(true)]
    public abstract class AsymmetricSignatureDeformatter
    {
        //
        // protected constructors
        //

        protected AsymmetricSignatureDeformatter() { }

        //
        // public methods
        //

        abstract public void SetKey(AsymmetricAlgorithm key);
        public abstract void SetHashAlgorithm(String strName);

        public virtual bool VerifySignature(HashAlgorithm hash, byte[] rgbSignature)
        {
            if (hash == null)
                throw new ArgumentNullException("hash");
            Contract.EndContractBlock();
            SetHashAlgorithm(hash.ToString());
            return VerifySignature(hash.Hash, rgbSignature);
        }

        public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);
    }
}
