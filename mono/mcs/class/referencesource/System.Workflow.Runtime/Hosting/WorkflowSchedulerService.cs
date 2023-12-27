using System;
using System.Threading;

namespace System.Workflow.Runtime.Hosting
{
    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public abstract class WorkflowSchedulerService : WorkflowRuntimeService
    {
        protected internal abstract void Schedule(WaitCallback callback, Guid workflowInstanceId);
        protected internal abstract void Schedule(
            WaitCallback callback,
            Guid workflowInstanceId,
            DateTime whenUtc,
            Guid timerId
        );
        protected internal abstract void Cancel(Guid timerId);
    }
}
