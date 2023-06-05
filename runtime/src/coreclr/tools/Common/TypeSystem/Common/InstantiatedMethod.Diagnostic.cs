// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial public sealed class InstantiatedMethod
    {
        public override string DiagnosticName
        {
            get { return _methodDef.DiagnosticName; }
        }
    }
}
