//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Net;
    using System.Net.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;

    public sealed partial class NetNamedPipeSecurityElement : ServiceModelConfigurationElement
    {
        [ConfigurationProperty(
            ConfigurationStrings.Mode,
            DefaultValue = NetNamedPipeSecurity.DefaultMode
        )]
        [ServiceModelEnumValidator(typeof(NetNamedPipeSecurityModeHelper))]
        public NetNamedPipeSecurityMode Mode
        {
            get { return (NetNamedPipeSecurityMode)base[ConfigurationStrings.Mode]; }
            set { base[ConfigurationStrings.Mode] = value; }
        }

        [ConfigurationProperty(ConfigurationStrings.Transport)]
        public NamedPipeTransportSecurityElement Transport
        {
            get { return (NamedPipeTransportSecurityElement)base[ConfigurationStrings.Transport]; }
        }

        internal void ApplyConfiguration(NetNamedPipeSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Transport.ApplyConfiguration(security.Transport);
        }

        internal void InitializeFrom(NetNamedPipeSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            SetPropertyValueIfNotDefaultValue(ConfigurationStrings.Mode, security.Mode);
            this.Transport.InitializeFrom(security.Transport);
        }
    }
}
