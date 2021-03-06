// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;

namespace Microsoft.TestCommon
{
    /// <summary>
    /// Preserves the current <see cref="SynchronizationContext"/>. Use this attribute on
    /// tests which modify the current <see cref="SynchronizationContext"/>.
    /// </summary>
    public class PreserveSyncContextAttribute : Xunit.Sdk.BeforeAfterTestAttribute
    {
        private SynchronizationContext _syncContext;

        public override void Before(System.Reflection.MethodInfo methodUnderTest)
        {
            _syncContext = SynchronizationContext.Current;
        }

        public override void After(System.Reflection.MethodInfo methodUnderTest)
        {
            SynchronizationContext.SetSynchronizationContext(_syncContext);
        }
    }
}
