// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Configuration
{
    partial
    /// <summary>
    /// NetHttpsBindingCollectionElement for NetHttpsBinding
    /// </summary>
    public class NetHttpsBindingCollectionElement
        : StandardBindingCollectionElement<NetHttpsBinding, NetHttpsBindingElement>
    {
        internal static NetHttpsBindingCollectionElement GetBindingCollectionElement()
        {
            return (NetHttpsBindingCollectionElement)
                ConfigurationHelpers.GetBindingCollectionElement(
                    ConfigurationStrings.NetHttpsBindingCollectionElementName
                );
        }
    }
}
