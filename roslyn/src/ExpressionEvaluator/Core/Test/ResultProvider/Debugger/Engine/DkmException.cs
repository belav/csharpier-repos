// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

#region Assembly Microsoft.VisualStudio.Debugger.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// References\Debugger\Concord\Microsoft.VisualStudio.Debugger.Engine.dll

#endregion

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Debugger
{
    //
    // Summary:
    //     Base exception class for all exceptions within this API.
    [DebuggerDisplay("\\{DkmException Code={Code,h}\\}")]
    [Serializable]
    public class DkmException : ApplicationException
    {
        private readonly DkmExceptionCode _code;

        //
        // Summary:
        //     Create a new exception instance. To enable native-interop scenarios, this exception
        //     system is error code based, so there is no exception string.
        //
        // Parameters:
        //   code:
        //     The HRESULT code for this exception. Using HRESULT values that are defined outside
        //     the range of this enumerator are acceptable, but not encouraged.
        public DkmException(DkmExceptionCode code)
        {
            _code = code;
        }

        protected DkmException(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Provides the DkmExcepionCode for this exception
        public DkmExceptionCode Code { get { return _code; } }
    }
}
