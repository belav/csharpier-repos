namespace System.Workflow.ComponentModel.Serialization
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Design;
    using System.Xml;
    using System.Xml.Serialization;

    #region Element deserialization hooks

    #region WorkflowMarkupElementEventArgs
    internal sealed class WorkflowMarkupElementEventArgs : EventArgs
    {
        private XmlReader reader = null;

        internal WorkflowMarkupElementEventArgs(XmlReader reader)
        {
            this.reader = reader;
        }

        public XmlReader XmlReader
        {
            get { return this.reader; }
        }
    }
    #endregion

    #endregion
}
