//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public class KerberosSecurityTokenParameters : SecurityTokenParameters
    {
        protected KerberosSecurityTokenParameters(KerberosSecurityTokenParameters other)
            : base(other)
        {
            // empty
        }

        public KerberosSecurityTokenParameters()
            : base()
        {
            this.InclusionMode = SecurityTokenInclusionMode.Once;
        }

        protected internal override bool HasAsymmetricKey
        {
            get { return false; }
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

        protected override SecurityTokenParameters CloneCore()
        {
            return new KerberosSecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            return base.CreateKeyIdentifierClause<
                KerberosTicketHashKeyIdentifierClause,
                LocalIdKeyIdentifierClause
            >(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            requirement.TokenType = SecurityTokenTypes.Kerberos;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.RequireCryptographicToken = true;
        }
    }
}
