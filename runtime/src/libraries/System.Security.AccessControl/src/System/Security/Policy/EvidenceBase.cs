// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security.Policy
{
    partial public abstract class EvidenceBase
    {
        protected EvidenceBase() { }

        public virtual EvidenceBase? Clone()
        {
            return default(EvidenceBase);
        }
    }
}
