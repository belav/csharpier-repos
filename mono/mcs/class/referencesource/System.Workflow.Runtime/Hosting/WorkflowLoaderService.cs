#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Xml;

#endregion


namespace System.Workflow.Runtime.Hosting
{
    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public abstract class WorkflowLoaderService : WorkflowRuntimeService
    {
        protected internal abstract Activity CreateInstance(Type workflowType);
        protected internal abstract Activity CreateInstance(
            XmlReader workflowDefinitionReader,
            XmlReader rulesReader
        );
    }
}
