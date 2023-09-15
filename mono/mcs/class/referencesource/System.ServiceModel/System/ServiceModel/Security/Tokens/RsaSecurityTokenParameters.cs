//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="UserNameSecurityTokenParameters.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public class RsaSecurityTokenParameters : SecurityTokenParameters
    {
        protected RsaSecurityTokenParameters(RsaSecurityTokenParameters other)
            : base(other)
        {
            this.InclusionMode = SecurityTokenInclusionMode.Never;
        }

        public RsaSecurityTokenParameters()
            : base()
        {
            this.InclusionMode = SecurityTokenInclusionMode.Never;
        }

        protected internal override bool HasAsymmetricKey
        {
            get { return true; }
        }

        protected internal override bool SupportsClientAuthentication
        {
            get { return true; }
        }
        protected internal override bool SupportsServerAuthentication
        {
            get { return true; }
        }
        protected internal override bool SupportsClientWindowsIdentity
        {
            get { return false; }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new RsaSecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            return this.CreateKeyIdentifierClause<RsaKeyIdentifierClause, RsaKeyIdentifierClause>(
                token,
                referenceStyle
            );
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            requirement.TokenType = SecurityTokenTypes.Rsa;
            requirement.RequireCryptographicToken = true;
            requirement.KeyType = SecurityKeyType.AsymmetricKey;
        }
    }
}
