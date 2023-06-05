// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    // Implements canonicalization handling for TypeDefs
    public class DefType
    {
        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            return this;
        }
    }
}
