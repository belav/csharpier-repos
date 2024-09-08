//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    class SecurityContextSecurityTokenParameters : SecurityTokenParameters
    {
        protected SecurityContextSecurityTokenParameters(
            SecurityContextSecurityTokenParameters other
        )
            : base(other)
        {
            // empty
        }

        public SecurityContextSecurityTokenParameters()
            : base()
        {
            this.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
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
            get { return true; }
        }

        protected internal override bool HasAsymmetricKey
        {
            get { return false; }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new SecurityContextSecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            return base.CreateKeyIdentifierClause<
                SecurityContextKeyIdentifierClause,
                LocalIdKeyIdentifierClause
            >(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            requirement.TokenType = ServiceModelSecurityTokenTypes.SecurityContext;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.RequireCryptographicToken = true;
        }
    }
}
