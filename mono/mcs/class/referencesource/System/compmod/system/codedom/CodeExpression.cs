//------------------------------------------------------------------------------
// <copyright file="CodeExpression.cs" company="Microsoft">
//
// <OWNER>petes</OWNER>
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
    ///       Represents a code expression.
    ///    </para>
    /// </devdoc>
    [ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), Serializable]
    public class CodeExpression : CodeObject { }
}
