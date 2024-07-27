//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;

    public partial class NetMsmqBindingCollectionElement
        : StandardBindingCollectionElement<NetMsmqBinding, NetMsmqBindingElement>
    {
        internal static NetMsmqBindingCollectionElement GetBindingCollectionElement()
        {
            return (NetMsmqBindingCollectionElement)
                ConfigurationHelpers.GetBindingCollectionElement(
                    ConfigurationStrings.NetMsmqBindingCollectionElementName
                );
        }
    }
}
