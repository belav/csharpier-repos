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
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.Text;

    public partial class MexNamedPipeBindingCollectionElement
        : MexBindingBindingCollectionElement<CustomBinding, MexNamedPipeBindingElement>
    {
        internal static MexNamedPipeBindingCollectionElement GetBindingCollectionElement()
        {
            return (MexNamedPipeBindingCollectionElement)
                ConfigurationHelpers.GetBindingCollectionElement(
                    ConfigurationStrings.MexNamedPipeBindingCollectionElementName
                );
        }

        protected internal override Binding GetDefault()
        {
            return MetadataExchangeBindings.GetBindingForScheme(Uri.UriSchemeNetPipe);
        }
    }
}
