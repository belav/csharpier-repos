#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime.Configuration;
using System.Workflow.Runtime.DebugEngine;
using System.Workflow.Runtime.Hosting;
using System.Workflow.Runtime.Tracking;
using System.Xml;

#endregion

namespace System.Workflow.Runtime
{
    internal sealed class CreationContext
    {
        internal Type Type;
        internal XmlReader XomlReader;
        internal XmlReader RulesReader;
        internal WorkflowExecutor InvokerExecutor;
        internal string InvokeActivityID;
        internal Dictionary<string, object> Args;
        internal bool IsActivation;
        internal bool Created;

        internal CreationContext(
            Type type,
            WorkflowExecutor invokerExec,
            string invokeActivityID,
            Dictionary<string, object> args
        )
        {
            Type = type;
            InvokerExecutor = invokerExec;
            InvokeActivityID = invokeActivityID;
            Args = args;
            IsActivation = true;
        }

        internal CreationContext(
            XmlReader xomlReader,
            XmlReader rulesReader,
            Dictionary<string, object> args
        )
        {
            XomlReader = xomlReader;
            RulesReader = rulesReader;
            InvokerExecutor = null;
            InvokeActivityID = null;
            Args = args;
            IsActivation = true;
        }
    }
}
