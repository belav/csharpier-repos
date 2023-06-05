// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Internal.TypeSystem
{
    partial public class ArrayMethod
    {
        public override string DiagnosticName
        {
            get
            {
                // The ArrayMethod.Name property is guaranteed to not throw
                return Name;
            }
        }
    }
}
