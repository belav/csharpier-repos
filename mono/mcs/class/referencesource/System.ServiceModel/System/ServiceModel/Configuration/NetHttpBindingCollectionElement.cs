// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel.Configuration
{
    partial
    /// <summary>
    /// NetHttpBindingCollectionElement for NetHttpBinding
    /// </summary>
    public class NetHttpBindingCollectionElement
        : StandardBindingCollectionElement<NetHttpBinding, NetHttpBindingElement>
    {
        internal static NetHttpBindingCollectionElement GetBindingCollectionElement()
        {
            return (NetHttpBindingCollectionElement)
                ConfigurationHelpers.GetBindingCollectionElement(
                    ConfigurationStrings.NetHttpBindingCollectionElementName
                );
        }
    }
}
