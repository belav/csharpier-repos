//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    // CSD Dev Framework#417: marker interface for IServiceBehaviors/Bindings that support context at the "app layer"
    interface IContextSessionProvider { }
}
