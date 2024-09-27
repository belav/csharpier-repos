#pragma warning disable 1634, 1691
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime.Hosting;
using System.Xml;
using SES = System.EnterpriseServices;

namespace System.Workflow.Runtime
{
    /// <summary>
    /// Internal events for workflow instances.
    /// </summary>
    [Serializable]
    internal enum WorkflowEventInternal
    {
        Created = 0,
        Completing,
        Completed,
        SchedulerEmpty,
        Idle,
        Suspending,
        Suspended,
        Resuming,
        Resumed,
        Persisting,
        Persisted,
        Unloading,
        Unloaded,
        Loaded,
        Exception,
        Terminating,
        Terminated,
        Aborting,
        Aborted,
        Runnable,
        Executing,
        NotExecuting,
        UserTrackPoint,
        ActivityStatusChange,
        ActivityStateCreated,
        HandlerEntered,
        HandlerExited,
        DynamicChangeBegin,
        DynamicChangeRollback,
        DynamicChangeCommit,
        Creating,
        Starting,
        Started,
        Changed,
        HandlerInvoking,
        HandlerInvoked,
        ActivityExecuting,
        Loading,
    }
}
