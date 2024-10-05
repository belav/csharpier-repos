//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    interface IProvideChannelBuilderSettings
    {
        ServiceChannelFactory ServiceChannelFactoryReadWrite { get; }
        ServiceChannelFactory ServiceChannelFactoryReadOnly { get; }
        KeyedByTypeCollection<IEndpointBehavior> Behaviors { get; }
        ServiceChannel ServiceChannel { get; }
    }
}
