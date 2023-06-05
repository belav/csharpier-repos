// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;

using Internal.Runtime;
using Internal.Runtime.Augments;

namespace Internal.TypeSystem
{
    partial public class CanonBaseType : DefType { }

    partial internal class CanonType
    {
        partial void Initialize()
        {
            SetRuntimeTypeHandleUnsafe(RuntimeAugments.GetCanonType(CanonTypeKind.NormalCanon));
        }
    }

    partial internal class UniversalCanonType
    {
        partial void Initialize()
        {
            SetRuntimeTypeHandleUnsafe(RuntimeAugments.GetCanonType(CanonTypeKind.UniversalCanon));
        }
    }
}
