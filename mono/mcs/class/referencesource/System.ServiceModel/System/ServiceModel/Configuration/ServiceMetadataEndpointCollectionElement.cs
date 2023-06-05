//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.ServiceModel;
    using System.Globalization;
    using System.ServiceModel.Description;

    partial public class ServiceMetadataEndpointCollectionElement
        : StandardEndpointCollectionElement<
            ServiceMetadataEndpoint,
            ServiceMetadataEndpointElement
        > { }
}
