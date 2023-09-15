//------------------------------------------------------------------------------
// <copyright file="CodeParser.cs" company="Microsoft">
//
// <OWNER>petes</OWNER>
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.CodeDom.Compiler
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides a code parsing abstract base class.
    ///    </para>
    /// </devdoc>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public abstract class CodeParser : ICodeParser
    {
        /// <devdoc>
        ///    <para>
        ///       Compiles the given text stream into a CodeCompile unit.
        ///    </para>
        /// </devdoc>
        public abstract CodeCompileUnit Parse(TextReader codeStream);
    }
}
