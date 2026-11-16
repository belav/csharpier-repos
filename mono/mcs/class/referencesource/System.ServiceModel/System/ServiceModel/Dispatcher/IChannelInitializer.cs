//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public interface IChannelInitializer
    {
        void Initialize(IClientChannel channel);
    }
}
