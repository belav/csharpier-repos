//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public interface IOperationBehavior
    {
        void Validate(OperationDescription operationDescription);
        void ApplyDispatchBehavior(
            OperationDescription operationDescription,
            DispatchOperation dispatchOperation
        );
        void ApplyClientBehavior(
            OperationDescription operationDescription,
            ClientOperation clientOperation
        );
        void AddBindingParameters(
            OperationDescription operationDescription,
            BindingParameterCollection bindingParameters
        );
    }
}
