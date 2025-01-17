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

    public abstract class SecurityTokenParameters
    {
        internal const SecurityTokenInclusionMode defaultInclusionMode =
            SecurityTokenInclusionMode.AlwaysToRecipient;
        internal const SecurityTokenReferenceStyle defaultReferenceStyle =
            SecurityTokenReferenceStyle.Internal;
        internal const bool defaultRequireDerivedKeys = true;

        SecurityTokenInclusionMode inclusionMode = defaultInclusionMode;
        SecurityTokenReferenceStyle referenceStyle = defaultReferenceStyle;
        bool requireDerivedKeys = defaultRequireDerivedKeys;

        protected SecurityTokenParameters(SecurityTokenParameters other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            this.requireDerivedKeys = other.requireDerivedKeys;
            this.inclusionMode = other.inclusionMode;
            this.referenceStyle = other.referenceStyle;
        }

        protected SecurityTokenParameters()
        {
            // empty
        }

        protected internal abstract bool HasAsymmetricKey { get; }

        public SecurityTokenInclusionMode InclusionMode
        {
            get { return this.inclusionMode; }
            set
            {
                SecurityTokenInclusionModeHelper.Validate(value);
                this.inclusionMode = value;
            }
        }

        public SecurityTokenReferenceStyle ReferenceStyle
        {
            get { return this.referenceStyle; }
            set
            {
                TokenReferenceStyleHelper.Validate(value);
                this.referenceStyle = value;
            }
        }

        public bool RequireDerivedKeys
        {
            get { return this.requireDerivedKeys; }
            set { this.requireDerivedKeys = value; }
        }

        protected internal abstract bool SupportsClientAuthentication { get; }
        protected internal abstract bool SupportsServerAuthentication { get; }
        protected internal abstract bool SupportsClientWindowsIdentity { get; }

        public SecurityTokenParameters Clone()
        {
            SecurityTokenParameters result = this.CloneCore();

            if (result == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(
                        SR.GetString(
                            SR.SecurityTokenParametersCloneInvalidResult,
                            this.GetType().ToString()
                        )
                    )
                );

            return result;
        }

        protected abstract SecurityTokenParameters CloneCore();

        protected internal abstract SecurityKeyIdentifierClause CreateKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        );

        protected internal abstract void InitializeSecurityTokenRequirement(
            SecurityTokenRequirement requirement
        );

        internal SecurityKeyIdentifierClause CreateKeyIdentifierClause<
            TExternalClause,
            TInternalClause
        >(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
            where TExternalClause : SecurityKeyIdentifierClause
            where TInternalClause : SecurityKeyIdentifierClause
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            SecurityKeyIdentifierClause result;

            switch (referenceStyle)
            {
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new NotSupportedException(
                            SR.GetString(
                                SR.TokenDoesNotSupportKeyIdentifierClauseCreation,
                                token.GetType().Name,
                                referenceStyle
                            )
                        )
                    );
                case SecurityTokenReferenceStyle.External:
                    result = token.CreateKeyIdentifierClause<TExternalClause>();
                    break;
                case SecurityTokenReferenceStyle.Internal:
                    result = token.CreateKeyIdentifierClause<TInternalClause>();
                    break;
            }

            return result;
        }

        internal SecurityKeyIdentifierClause CreateGenericXmlTokenKeyIdentifierClause(
            SecurityToken token,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            GenericXmlSecurityToken xmlToken = token as GenericXmlSecurityToken;
            if (xmlToken != null)
            {
                if (
                    referenceStyle == SecurityTokenReferenceStyle.Internal
                    && xmlToken.InternalTokenReference != null
                )
                    return xmlToken.InternalTokenReference;

                if (
                    referenceStyle == SecurityTokenReferenceStyle.External
                    && xmlToken.ExternalTokenReference != null
                )
                    return xmlToken.ExternalTokenReference;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                new MessageSecurityException(SR.GetString(SR.UnableToCreateTokenReference))
            );
        }

        protected internal virtual bool MatchesKeyIdentifierClause(
            SecurityToken token,
            SecurityKeyIdentifierClause keyIdentifierClause,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            if (token is GenericXmlSecurityToken)
            {
                return MatchesGenericXmlTokenKeyIdentifierClause(
                    token,
                    keyIdentifierClause,
                    referenceStyle
                );
            }

            bool result;

            switch (referenceStyle)
            {
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new NotSupportedException(
                            SR.GetString(
                                SR.TokenDoesNotSupportKeyIdentifierClauseCreation,
                                token.GetType().Name,
                                referenceStyle
                            )
                        )
                    );
                case SecurityTokenReferenceStyle.External:
                    if (keyIdentifierClause is LocalIdKeyIdentifierClause)
                        result = false;
                    else
                        result = token.MatchesKeyIdentifierClause(keyIdentifierClause);
                    break;
                case SecurityTokenReferenceStyle.Internal:
                    result = token.MatchesKeyIdentifierClause(keyIdentifierClause);
                    break;
            }

            return result;
        }

        internal bool MatchesGenericXmlTokenKeyIdentifierClause(
            SecurityToken token,
            SecurityKeyIdentifierClause keyIdentifierClause,
            SecurityTokenReferenceStyle referenceStyle
        )
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            bool result;

            GenericXmlSecurityToken xmlToken = token as GenericXmlSecurityToken;

            if (xmlToken == null)
                result = false;
            else if (
                referenceStyle == SecurityTokenReferenceStyle.External
                && xmlToken.ExternalTokenReference != null
            )
                result = xmlToken.ExternalTokenReference.Matches(keyIdentifierClause);
            else if (referenceStyle == SecurityTokenReferenceStyle.Internal)
                result = xmlToken.MatchesKeyIdentifierClause(keyIdentifierClause);
            else
                result = false;

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(
                String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString())
            );
            sb.AppendLine(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "InclusionMode: {0}",
                    this.inclusionMode.ToString()
                )
            );
            sb.AppendLine(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "ReferenceStyle: {0}",
                    this.referenceStyle.ToString()
                )
            );
            sb.Append(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "RequireDerivedKeys: {0}",
                    this.requireDerivedKeys.ToString()
                )
            );

            return sb.ToString();
        }
    }
}
