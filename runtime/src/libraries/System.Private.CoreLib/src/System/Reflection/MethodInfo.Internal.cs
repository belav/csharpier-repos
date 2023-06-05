// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection
{
    partial public abstract class MethodInfo : MethodBase
    {
        internal virtual int GenericParameterCount => GetGenericArguments().Length;
    }
}
