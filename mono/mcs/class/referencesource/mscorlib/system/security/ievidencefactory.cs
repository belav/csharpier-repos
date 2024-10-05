// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
//  IEvidenceFactory.cs
//
// <OWNER>ShawnFa</OWNER>
//

namespace System.Security
{
    using System;
    using System.Runtime.Remoting;
    using System.Security.Policy;

    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IEvidenceFactory
    {
#if FEATURE_CAS_POLICY
        Evidence Evidence { get; }
#endif // FEATURE_CAS_POLICY
    }
}
