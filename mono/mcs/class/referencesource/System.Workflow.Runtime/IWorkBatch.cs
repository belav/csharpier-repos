#pragma warning disable 1634, 1691
using System;
using System.Collections;
using System.Diagnostics;
using System.Transactions;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime;

namespace System.Workflow.Runtime
{
    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public interface IWorkBatch
    {
        void Add(IPendingWork work, object workItem);
    }
}
