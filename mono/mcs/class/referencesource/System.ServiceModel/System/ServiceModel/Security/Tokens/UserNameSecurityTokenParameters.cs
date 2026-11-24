//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Security.Tokens
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public class UserNameSecurityTokenParameters : SecurityTokenParameters
    {
        protected UserNameSecurityTokenParameters(UserNameSecurityTokenParameters other)
            : base(other)
        {
            base.RequireDerivedKeys = false;
        }

        public UserNameSecurityTokenParameters()
            : base()
        {
            base.RequireDerivedKeys = false;
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
            get { return false; }
        }
        protected internal override bool SupportsClientWindowsIdentity
        {
            get { return true; }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new UserNameSecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            return this.CreateKeyIdentifierClause<
                SecurityKeyIdentifierClause,
                LocalIdKeyIdentifierClause
            >(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            requirement.TokenType = SecurityTokenTypes.UserName;
            requirement.RequireCryptographicToken = false;
        }
    }
}
