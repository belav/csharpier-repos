namespace System.Workflow.ComponentModel.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using System.Workflow.ComponentModel.Design;
    using System.Xml;

    #region Class BindMarkupExtensionSerializer
    internal class BindMarkupExtensionSerializer : MarkupExtensionSerializer
    {
        protected override InstanceDescriptor GetInstanceDescriptor(
            WorkflowMarkupSerializationManager serializationManager,
            object value
        )
        {
            ActivityBind activityBind = value as ActivityBind;
            if (activityBind == null)
                throw new ArgumentException(
                    SR.GetString(SR.Error_UnexpectedArgumentType, typeof(ActivityBind).FullName),
                    "value"
                );
            return new InstanceDescriptor(
                typeof(ActivityBind).GetConstructor(new Type[] { typeof(string) }),
                new object[] { activityBind.Name }
            );
        }
    }
    #endregion
}
