//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Security.Tokens
{
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Text;

    public class SspiSecurityTokenParameters : SecurityTokenParameters
    {
        internal const bool defaultRequireCancellation = false;

        bool requireCancellation = defaultRequireCancellation;
        BindingContext issuerBindingContext;

        protected SspiSecurityTokenParameters(SspiSecurityTokenParameters other)
            : base(other)
        {
            this.requireCancellation = other.requireCancellation;
            if (other.issuerBindingContext != null)
            {
                this.issuerBindingContext = other.issuerBindingContext.Clone();
            }
        }

        public SspiSecurityTokenParameters()
            : this(defaultRequireCancellation)
        {
            // empty
        }

        public SspiSecurityTokenParameters(bool requireCancellation)
            : base()
        {
            this.requireCancellation = requireCancellation;
        }

        protected internal override bool HasAsymmetricKey
        {
            get { return false; }
        }

        public bool RequireCancellation
        {
            get { return this.requireCancellation; }
            set { this.requireCancellation = value; }
        }

        internal BindingContext IssuerBindingContext
        {
            get { return this.issuerBindingContext; }
            set
            {
                if (value != null)
                {
                    value = value.Clone();
                }
                this.issuerBindingContext = value;
            }
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
            return new SspiSecurityTokenParameters(this);
        }

        protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            if (token is GenericXmlSecurityToken)
                return base.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
            else
                return this.CreateKeyIdentifierClause<
                    SecurityContextKeyIdentifierClause,
                    LocalIdKeyIdentifierClause
                >(token, referenceStyle);
        }

        protected internal override void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        )
        {
            requirement.TokenType = ServiceModelSecurityTokenTypes.Spnego;
            requirement.RequireCryptographicToken = true;
            requirement.KeyType = SecurityKeyType.SymmetricKey;
            requirement.Properties[
                ServiceModelSecurityTokenRequirement.SupportSecurityContextCancellationProperty
            ] = this.RequireCancellation;
            if (this.IssuerBindingContext != null)
            {
                requirement.Properties[
                    ServiceModelSecurityTokenRequirement.IssuerBindingContextProperty
                ] = this.IssuerBindingContext.Clone();
            }
            requirement.Properties[
                ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty
            ] = this.Clone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.Append(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "RequireCancellation: {0}",
                    this.RequireCancellation.ToString()
                )
            );

            return sb.ToString();
        }
    }
}
