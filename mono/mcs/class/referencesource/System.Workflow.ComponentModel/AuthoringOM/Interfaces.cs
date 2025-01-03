namespace System.Workflow.ComponentModel
{
    #region Imports

    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Design;
    using System.Xml;
    using System.Xml.Serialization;

    #endregion

    // Interface for objects that support mining for workflow changes.
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public interface IWorkflowChangeDiff
    {
        IList<WorkflowChangeAction> Diff(object originalDefinition, object changedDefinition);
    }
}
