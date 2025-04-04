// Copyright (c) Microsoft Corp., 2004. All rights reserved.
#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Hosting;
using System.Xml;
using Microsoft.Win32;
#endregion

namespace System.Workflow.Runtime.DebugEngine
{
    #region Interface IWorkflowDebugger

    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public interface IWorkflowDebugger
    {
        void InstanceCreated(Guid programId, Guid instanceId, Guid scheduleTypeId);
        void InstanceDynamicallyUpdated(Guid programId, Guid instanceId, Guid scheduleTypeId);
        void InstanceCompleted(Guid programId, Guid instanceId);
        void BeforeActivityStatusChanged(
            Guid programId,
            Guid scheduleTypeId,
            Guid instanceId,
            string activityQualifiedName,
            string hierarchicalActivityId,
            ActivityExecutionStatus status,
            int stateReaderId
        );
        void ActivityStatusChanged(
            Guid programId,
            Guid scheduleTypeId,
            Guid instanceId,
            string activityQualifiedName,
            string hierarchicalActivityId,
            ActivityExecutionStatus status,
            int stateReaderId
        );
        void SetInitialActivityStatus(
            Guid programId,
            Guid scheduleTypeId,
            Guid instanceId,
            string activityQualifiedName,
            string hierarchicalActivityId,
            ActivityExecutionStatus status,
            int stateReaderId
        );
        void ScheduleTypeLoaded(
            Guid programId,
            Guid scheduleTypeId,
            string assemblyFullName,
            string fileName,
            string md5Digest,
            bool isDynamic,
            string scheduleNamespace,
            string scheduleName,
            string workflowMarkup
        );
        void UpdateHandlerMethodsForActivity(
            Guid programId,
            Guid scheduleTypeId,
            string activityQualifiedName,
            List<ActivityHandlerDescriptor> handlerMethods
        );
        void AssemblyLoaded(Guid programId, string assemblyPath, bool fromGlobalAssemblyCache);
        void HandlerInvoked(
            Guid programId,
            Guid instanceId,
            int threadId,
            string activityQualifiedName
        );
        void BeforeHandlerInvoked(
            Guid programId,
            Guid scheduleTypeId,
            string activityQualifiedName,
            ActivityHandlerDescriptor handlerMethod
        );
    }

    #endregion
}
