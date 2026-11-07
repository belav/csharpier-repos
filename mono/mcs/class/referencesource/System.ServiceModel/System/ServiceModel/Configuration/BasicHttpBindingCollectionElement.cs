//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;

    public partial class BasicHttpBindingCollectionElement
        : StandardBindingCollectionElement<BasicHttpBinding, BasicHttpBindingElement>
    {
        internal static BasicHttpBindingCollectionElement GetBindingCollectionElement()
        {
            return (BasicHttpBindingCollectionElement)
                ConfigurationHelpers.GetBindingCollectionElement(
                    ConfigurationStrings.BasicHttpBindingCollectionElementName
                );
        }
    }
}
