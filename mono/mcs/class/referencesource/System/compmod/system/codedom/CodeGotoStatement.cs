//------------------------------------------------------------------------------
// <copyright file="CodeGotoStatement.cs" company="Microsoft">
//
// <OWNER>Microsoft</OWNER>
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.CodeDom
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Serializable]
    public class CodeGotoStatement : CodeStatement
    {
        private string label;

        public CodeGotoStatement() { }

        public CodeGotoStatement(string label)
        {
            Label = label;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Label
        {
            get { return label; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                this.label = value;
            }
        }
    }
}
