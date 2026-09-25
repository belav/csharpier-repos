//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using System.ServiceModel.Transactions;
    using System.Transactions;

    internal interface ITransactionChannelManager
    {
        TransactionProtocol TransactionProtocol { get; set; }
        TransactionFlowOption FlowIssuedTokens { get; set; }
        IDictionary<DirectionalAction, TransactionFlowOption> Dictionary { get; }
        TransactionFlowOption GetTransaction(MessageDirection direction, string action);
        SecurityStandardsManager StandardsManager { get; }
    }
}
