// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    // Holds code for canonicalization of metadata types
    public class MetadataType
    {
        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            return false;
        }
    }
}
