// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Security
{
    partial public abstract class SecurityState
    {
        protected SecurityState() { }

        public abstract void EnsureState();

        public bool IsStateAvailable()
        {
            return false;
        }
    }
}
