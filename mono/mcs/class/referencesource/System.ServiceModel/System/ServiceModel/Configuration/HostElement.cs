//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.Configuration;

    partial public sealed class HostElement : ConfigurationElement
    {
        public HostElement()
            : base() { }

        [ConfigurationProperty(
            ConfigurationStrings.BaseAddresses,
            Options = ConfigurationPropertyOptions.None
        )]
        public BaseAddressElementCollection BaseAddresses
        {
            get { return (BaseAddressElementCollection)base[ConfigurationStrings.BaseAddresses]; }
        }

        [ConfigurationProperty(
            ConfigurationStrings.Timeouts,
            Options = ConfigurationPropertyOptions.None
        )]
        public HostTimeoutsElement Timeouts
        {
            get { return (HostTimeoutsElement)base[ConfigurationStrings.Timeouts]; }
        }
    }
}
