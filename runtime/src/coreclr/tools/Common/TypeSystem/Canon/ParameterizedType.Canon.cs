// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    // Holds code for canonicalizing a parameterized type
    public class ParameterizedType
    {
        public sealed override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            return ParameterType.IsCanonicalSubtype(policy);
        }
    }
}
