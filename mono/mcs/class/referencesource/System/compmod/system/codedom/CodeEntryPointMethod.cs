//------------------------------------------------------------------------------
// <copyright file="CodeEntryPointMethod.cs" company="Microsoft">
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
    ///    <para>
    ///       Represents a class method that is the entry point
    ///    </para>
    /// </devdoc>
    [ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Serializable]
    public class CodeEntryPointMethod : CodeMemberMethod
    {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeEntryPointMethod() { }
    }
}
