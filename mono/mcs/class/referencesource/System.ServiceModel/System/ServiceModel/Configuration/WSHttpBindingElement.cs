//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Text;

    public partial class WSHttpBindingElement : WSHttpBindingBaseElement
    {
        public WSHttpBindingElement(string name)
            : base(name) { }

        public WSHttpBindingElement()
            : this(null) { }

        protected override Type BindingElementType
        {
            get { return typeof(WSHttpBinding); }
        }

        [ConfigurationProperty(
            ConfigurationStrings.AllowCookies,
            DefaultValue = HttpTransportDefaults.AllowCookies
        )]
        public bool AllowCookies
        {
            get { return (bool)base[ConfigurationStrings.AllowCookies]; }
            set { base[ConfigurationStrings.AllowCookies] = value; }
        }

        [ConfigurationProperty(ConfigurationStrings.Security)]
        public WSHttpSecurityElement Security
        {
            get { return (WSHttpSecurityElement)base[ConfigurationStrings.Security]; }
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            WSHttpBinding wspBinding = (WSHttpBinding)binding;

            SetPropertyValueIfNotDefaultValue(
                ConfigurationStrings.AllowCookies,
                wspBinding.AllowCookies
            );
            this.Security.InitializeFrom(wspBinding.Security);
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            base.OnApplyConfiguration(binding);
            WSHttpBinding wspBinding = (WSHttpBinding)binding;

            wspBinding.AllowCookies = this.AllowCookies;
            this.Security.ApplyConfiguration(wspBinding.Security);
        }
    }
}
