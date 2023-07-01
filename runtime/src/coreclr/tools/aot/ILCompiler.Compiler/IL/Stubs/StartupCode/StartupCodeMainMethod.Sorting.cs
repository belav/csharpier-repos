using Debug = System.Diagnostics.Debug;
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Internal.TypeSystem;

namespace Internal.IL.Stubs.StartupCode
{
    public partial class StartupCodeMainMethod
    {
        protected override int ClassCode => -304225481;

        protected override int CompareToImpl(MethodDesc other, TypeSystemComparer comparer)
        {
            // Should be a singleton
            Debug.Assert(this == other);
            return 0;
        }

        private partial class MainMethodWrapper
        {
            protected override int ClassCode => -215672259;

            protected override int CompareToImpl(MethodDesc other, TypeSystemComparer comparer)
            {
                // Should be a singleton
                Debug.Assert(this == other);
                return 0;
            }
        }
    }
}
