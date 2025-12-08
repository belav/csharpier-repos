//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading;
    using Microsoft.Win32;

    internal class MonikerSyntaxException : COMException
    {
        internal MonikerSyntaxException(string message)
            : base(message, HR.MK_E_SYNTAX) { }
    }
}
