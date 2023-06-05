// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Debug = System.Diagnostics.Debug;

namespace Internal.TypeSystem
{
    partial
    // Functionality related to deterministic ordering of types
    public class CanonBaseType
    {
        protected internal sealed override int CompareToImpl(
            TypeDesc other,
            TypeSystemComparer comparer
        )
        {
            // These should be singletons
            Debug.Assert(this == other);
            return 0;
        }
    }

    partial internal class CanonType
    {
        protected internal override int ClassCode => 46114331;
    }

    partial internal class UniversalCanonType
    {
        protected internal override int ClassCode => 1687626054;
    }
}
