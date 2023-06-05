// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial
    // Functionality related to deterministic ordering of types
    public class SignatureVariable
    {
        protected internal sealed override int CompareToImpl(
            TypeDesc other,
            TypeSystemComparer comparer
        )
        {
            return ((SignatureVariable)other).Index - Index;
        }
    }

    partial public class SignatureTypeVariable
    {
        protected internal override int ClassCode
        {
            get { return 1042124696; }
        }
    }

    partial public class SignatureMethodVariable
    {
        protected internal override int ClassCode
        {
            get { return 144542889; }
        }
    }
}
