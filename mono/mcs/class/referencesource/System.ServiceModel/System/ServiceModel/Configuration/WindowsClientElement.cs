//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Xml;

    public sealed partial class WindowsClientElement : ConfigurationElement
    {
        public WindowsClientElement() { }

        [ConfigurationProperty(
            ConfigurationStrings.AllowNtlm,
            DefaultValue = SspiSecurityTokenProvider.DefaultAllowNtlm
        )]
        public bool AllowNtlm
        {
            get { return (bool)base[ConfigurationStrings.AllowNtlm]; }
            set { base[ConfigurationStrings.AllowNtlm] = value; }
        }

        [ConfigurationProperty(
            ConfigurationStrings.AllowedImpersonationLevel,
            DefaultValue = WindowsClientCredential.DefaultImpersonationLevel
        )]
        [ServiceModelEnumValidator(typeof(TokenImpersonationLevelHelper))]
        public TokenImpersonationLevel AllowedImpersonationLevel
        {
            get
            {
                return (TokenImpersonationLevel)
                    base[ConfigurationStrings.AllowedImpersonationLevel];
            }
            set { base[ConfigurationStrings.AllowedImpersonationLevel] = value; }
        }

        public void Copy(WindowsClientElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ConfigurationErrorsException(SR.GetString(SR.ConfigReadOnly))
                );
            }
            if (null == from)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }

            this.AllowNtlm = from.AllowNtlm;
            this.AllowedImpersonationLevel = from.AllowedImpersonationLevel;
        }

        internal void ApplyConfiguration(WindowsClientCredential windows)
        {
            if (windows == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windows");
            }

            // To suppress AllowNtlm warning.
#pragma warning disable 618
            windows.AllowNtlm = this.AllowNtlm;
#pragma warning restore 618

            windows.AllowedImpersonationLevel = this.AllowedImpersonationLevel;
        }
    }
}
