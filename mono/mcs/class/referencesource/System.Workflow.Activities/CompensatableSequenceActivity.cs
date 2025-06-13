namespace System.Workflow.Activities
{
    #region Imports

    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Reflection;
    using System.Text;
    using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Design;

    #endregion

    [SRDescription(SR.CompensatableSequenceActivityDescription)]
    [ToolboxItem(typeof(ActivityToolboxItem))]
    [Designer(typeof(SequenceDesigner), typeof(IDesigner))]
    [ToolboxBitmap(typeof(CompensatableSequenceActivity), "Resources.Sequence.png")]
    [SRCategory(SR.Standard)]
    [Obsolete(
        "The System.Workflow.* types are deprecated.  Instead, please use the new types from System.Activities.*"
    )]
    public sealed class CompensatableSequenceActivity : SequenceActivity, ICompensatableActivity
    {
        #region Constructors

        public CompensatableSequenceActivity() { }

        public CompensatableSequenceActivity(string name)
            : base(name) { }

        #endregion

        #region ICompensatableActivity Members
        ActivityExecutionStatus ICompensatableActivity.Compensate(
            ActivityExecutionContext executionContext
        )
        {
            return ActivityExecutionStatus.Closed;
        }
        #endregion
    }
}
