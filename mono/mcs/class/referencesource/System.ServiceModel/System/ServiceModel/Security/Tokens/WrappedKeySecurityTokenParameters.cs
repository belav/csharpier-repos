//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------


namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    class WrappedKeySecurityTokenParameters : SecurityTokenParameters
    {
        protected WrappedKeySecurityTokenParameters(WrappedKeySecurityTokenParameters other)
            : base(other)
        {
            // empty
        }

        public WrappedKeySecurityTokenParameters()
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
            get { return false; }
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
            return new WrappedKeySecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            return base.CreateKeyIdentifierClause<
                EncryptedKeyHashIdentifierClause,
                LocalIdKeyIdentifierClause
            >(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            throw DiagnosticUtility
                .ExceptionUtility
                .ThrowHelperError(new NotImplementedException());
        }
    }
}
