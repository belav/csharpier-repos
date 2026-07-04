// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.JavaScript;
using Microsoft.CodeAnalysis;

namespace Microsoft.Interop.JavaScript
{
    internal record JSMarshallingInfo(MarshallingInfo Inner, JSTypeInfo TypeInfo) : MarshallingInfo
    {
        protected JSMarshallingInfo()
            : this(NoMarshallingInfo.Instance, new JSInvalidTypeInfo())
        {
            Inner = null;
        }

        public JSTypeFlags JSType { get; init; }
        public JSTypeFlags[] JSTypeArguments { get; init; }
    }

    internal sealed record JSMissingMarshallingInfo : JSMarshallingInfo
    {
        public JSMissingMarshallingInfo(JSTypeInfo typeInfo)
        {
            JSType = JSTypeFlags.Missing;
            TypeInfo = typeInfo;
        }
    }
}
