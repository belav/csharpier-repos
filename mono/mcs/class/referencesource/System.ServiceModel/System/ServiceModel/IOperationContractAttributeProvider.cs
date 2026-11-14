//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    using System.Net.Security;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel.Security;
    using System.Transactions;

    interface IOperationContractAttributeProvider
    {
        OperationContractAttribute GetOperationContractAttribute();
    }
}
