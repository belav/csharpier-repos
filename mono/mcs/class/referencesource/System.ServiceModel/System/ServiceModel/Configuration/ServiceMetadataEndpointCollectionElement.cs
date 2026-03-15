//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public partial class ServiceMetadataEndpointCollectionElement
        : StandardEndpointCollectionElement<
            ServiceMetadataEndpoint,
            ServiceMetadataEndpointElement
        > { }
}
