//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Security.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Text;
    using System.Xml;

    public abstract class ServiceModelSecurityTokenRequirement : SecurityTokenRequirement
    {
        protected const string Namespace =
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement";
        const string securityAlgorithmSuiteProperty = Namespace + "/SecurityAlgorithmSuite";
        const string securityBindingElementProperty = Namespace + "/SecurityBindingElement";
        const string issuerAddressProperty = Namespace + "/IssuerAddress";
        const string issuerBindingProperty = Namespace + "/IssuerBinding";
        const string secureConversationSecurityBindingElementProperty =
            Namespace + "/SecureConversationSecurityBindingElement";
        const string supportSecurityContextCancellationProperty =
            Namespace + "/SupportSecurityContextCancellation";
        const string messageSecurityVersionProperty = Namespace + "/MessageSecurityVersion";
        const string defaultMessageSecurityVersionProperty =
            Namespace + "/DefaultMessageSecurityVersion";
        const string issuerBindingContextProperty = Namespace + "/IssuerBindingContext";
        const string transportSchemeProperty = Namespace + "/TransportScheme";
        const string isInitiatorProperty = Namespace + "/IsInitiator";
        const string targetAddressProperty = Namespace + "/TargetAddress";
        const string viaProperty = Namespace + "/Via";
        const string listenUriProperty = Namespace + "/ListenUri";
        const string auditLogLocationProperty = Namespace + "/AuditLogLocation";
        const string suppressAuditFailureProperty = Namespace + "/SuppressAuditFailure";
        const string messageAuthenticationAuditLevelProperty =
            Namespace + "/MessageAuthenticationAuditLevel";
        const string isOutOfBandTokenProperty = Namespace + "/IsOutOfBandToken";
        const string preferSslCertificateAuthenticatorProperty =
            Namespace + "/PreferSslCertificateAuthenticator";

        // the following properties dont have top level OM properties but are part of the property bag
        const string supportingTokenAttachmentModeProperty =
            Namespace + "/SupportingTokenAttachmentMode";
        const string messageDirectionProperty = Namespace + "/MessageDirection";
        const string httpAuthenticationSchemeProperty = Namespace + "/HttpAuthenticationScheme";
        const string issuedSecurityTokenParametersProperty =
            Namespace + "/IssuedSecurityTokenParameters";
        const string privacyNoticeUriProperty = Namespace + "/PrivacyNoticeUri";
        const string privacyNoticeVersionProperty = Namespace + "/PrivacyNoticeVersion";
        const string duplexClientLocalAddressProperty = Namespace + "/DuplexClientLocalAddress";
        const string endpointFilterTableProperty = Namespace + "/EndpointFilterTable";
        const string channelParametersCollectionProperty =
            Namespace + "/ChannelParametersCollection";
        const string extendedProtectionPolicy = Namespace + "/ExtendedProtectionPolicy";

        const bool defaultSupportSecurityContextCancellation = false;

        protected ServiceModelSecurityTokenRequirement()
            : base()
        {
            this.Properties[SupportSecurityContextCancellationProperty] =
                defaultSupportSecurityContextCancellation;
        }

        public static string SecurityAlgorithmSuiteProperty
        {
            get { return securityAlgorithmSuiteProperty; }
        }
        public static string SecurityBindingElementProperty
        {
            get { return securityBindingElementProperty; }
        }
        public static string IssuerAddressProperty
        {
            get { return issuerAddressProperty; }
        }
        public static string IssuerBindingProperty
        {
            get { return issuerBindingProperty; }
        }
        public static string SecureConversationSecurityBindingElementProperty
        {
            get { return secureConversationSecurityBindingElementProperty; }
        }
        public static string SupportSecurityContextCancellationProperty
        {
            get { return supportSecurityContextCancellationProperty; }
        }
        public static string MessageSecurityVersionProperty
        {
            get { return messageSecurityVersionProperty; }
        }
        internal static string DefaultMessageSecurityVersionProperty
        {
            get { return defaultMessageSecurityVersionProperty; }
        }
        public static string IssuerBindingContextProperty
        {
            get { return issuerBindingContextProperty; }
        }
        public static string TransportSchemeProperty
        {
            get { return transportSchemeProperty; }
        }
        public static string IsInitiatorProperty
        {
            get { return isInitiatorProperty; }
        }
        public static string TargetAddressProperty
        {
            get { return targetAddressProperty; }
        }
        public static string ViaProperty
        {
            get { return viaProperty; }
        }
        public static string ListenUriProperty
        {
            get { return listenUriProperty; }
        }
        public static string AuditLogLocationProperty
        {
            get { return auditLogLocationProperty; }
        }
        public static string SuppressAuditFailureProperty
        {
            get { return suppressAuditFailureProperty; }
        }
        public static string MessageAuthenticationAuditLevelProperty
        {
            get { return messageAuthenticationAuditLevelProperty; }
        }
        public static string IsOutOfBandTokenProperty
        {
            get { return isOutOfBandTokenProperty; }
        }
        public static string PreferSslCertificateAuthenticatorProperty
        {
            get { return preferSslCertificateAuthenticatorProperty; }
        }

        public static string SupportingTokenAttachmentModeProperty
        {
            get { return supportingTokenAttachmentModeProperty; }
        }
        public static string MessageDirectionProperty
        {
            get { return messageDirectionProperty; }
        }
        public static string HttpAuthenticationSchemeProperty
        {
            get { return httpAuthenticationSchemeProperty; }
        }
        public static string IssuedSecurityTokenParametersProperty
        {
            get { return issuedSecurityTokenParametersProperty; }
        }
        public static string PrivacyNoticeUriProperty
        {
            get { return privacyNoticeUriProperty; }
        }
        public static string PrivacyNoticeVersionProperty
        {
            get { return privacyNoticeVersionProperty; }
        }
        public static string DuplexClientLocalAddressProperty
        {
            get { return duplexClientLocalAddressProperty; }
        }
        public static string EndpointFilterTableProperty
        {
            get { return endpointFilterTableProperty; }
        }
        public static string ChannelParametersCollectionProperty
        {
            get { return channelParametersCollectionProperty; }
        }
        public static string ExtendedProtectionPolicy
        {
            get { return extendedProtectionPolicy; }
        }

        public bool IsInitiator
        {
            get { return GetPropertyOrDefault<bool>(IsInitiatorProperty, false); }
        }

        public SecurityAlgorithmSuite SecurityAlgorithmSuite
        {
            get
            {
                return GetPropertyOrDefault<SecurityAlgorithmSuite>(
                    SecurityAlgorithmSuiteProperty,
                    null
                );
            }
            set { this.Properties[SecurityAlgorithmSuiteProperty] = value; }
        }

        public SecurityBindingElement SecurityBindingElement
        {
            get
            {
                return GetPropertyOrDefault<SecurityBindingElement>(
                    SecurityBindingElementProperty,
                    null
                );
            }
            set { this.Properties[SecurityBindingElementProperty] = value; }
        }

        public EndpointAddress IssuerAddress
        {
            get { return GetPropertyOrDefault<EndpointAddress>(IssuerAddressProperty, null); }
            set { this.Properties[IssuerAddressProperty] = value; }
        }

        public Binding IssuerBinding
        {
            get { return GetPropertyOrDefault<Binding>(IssuerBindingProperty, null); }
            set { this.Properties[IssuerBindingProperty] = value; }
        }

        public SecurityBindingElement SecureConversationSecurityBindingElement
        {
            get
            {
                return GetPropertyOrDefault<SecurityBindingElement>(
                    SecureConversationSecurityBindingElementProperty,
                    null
                );
            }
            set { this.Properties[SecureConversationSecurityBindingElementProperty] = value; }
        }

        public SecurityTokenVersion MessageSecurityVersion
        {
            get
            {
                return GetPropertyOrDefault<SecurityTokenVersion>(
                    MessageSecurityVersionProperty,
                    null
                );
            }
            set { this.Properties[MessageSecurityVersionProperty] = value; }
        }

        internal MessageSecurityVersion DefaultMessageSecurityVersion
        {
            get
            {
                MessageSecurityVersion messageSecurityVersion;
                return (
                    this.TryGetProperty<MessageSecurityVersion>(
                        DefaultMessageSecurityVersionProperty,
                        out messageSecurityVersion
                    )
                )
                    ? messageSecurityVersion
                    : null;
            }
            set { this.Properties[DefaultMessageSecurityVersionProperty] = (object)value; }
        }

        public string TransportScheme
        {
            get { return GetPropertyOrDefault<string>(TransportSchemeProperty, null); }
            set { this.Properties[TransportSchemeProperty] = value; }
        }

        internal bool SupportSecurityContextCancellation
        {
            get
            {
                return GetPropertyOrDefault<bool>(
                    SupportSecurityContextCancellationProperty,
                    defaultSupportSecurityContextCancellation
                );
            }
            set { this.Properties[SupportSecurityContextCancellationProperty] = value; }
        }

        internal EndpointAddress DuplexClientLocalAddress
        {
            get
            {
                return GetPropertyOrDefault<EndpointAddress>(
                    duplexClientLocalAddressProperty,
                    null
                );
            }
            set { this.Properties[duplexClientLocalAddressProperty] = value; }
        }

        internal TValue GetPropertyOrDefault<TValue>(string propertyName, TValue defaultValue)
        {
            TValue result;
            if (!TryGetProperty<TValue>(propertyName, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        internal string InternalToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(
                String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString())
            );
            foreach (string propertyName in this.Properties.Keys)
            {
                object propertyValue = this.Properties[propertyName];
                sb.AppendLine(
                    String.Format(CultureInfo.InvariantCulture, "PropertyName: {0}", propertyName)
                );
                sb.AppendLine(
                    String.Format(CultureInfo.InvariantCulture, "PropertyValue: {0}", propertyValue)
                );
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "---"));
            }
            return sb.ToString().Trim();
        }
    }
}
