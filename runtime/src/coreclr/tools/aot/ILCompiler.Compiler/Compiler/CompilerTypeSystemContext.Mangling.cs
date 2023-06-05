// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Internal.TypeSystem;

namespace ILCompiler
{
    partial
    // Contains functionality related to name mangling
    public class CompilerTypeSystemContext
    {
        partial private class BoxedValueType : IPrefixMangledType
        {
            TypeDesc IPrefixMangledType.BaseType
            {
                get { return ValueTypeRepresented; }
            }

            string IPrefixMangledType.Prefix
            {
                get { return "Boxed"; }
            }
        }

        partial private class GenericUnboxingThunk : IPrefixMangledMethod
        {
            MethodDesc IPrefixMangledMethod.BaseMethod
            {
                get { return _targetMethod; }
            }

            string IPrefixMangledMethod.Prefix
            {
                get { return "unbox"; }
            }
        }

        partial private class UnboxingThunk : IPrefixMangledMethod
        {
            MethodDesc IPrefixMangledMethod.BaseMethod
            {
                get { return _targetMethod; }
            }

            string IPrefixMangledMethod.Prefix
            {
                get { return "unbox"; }
            }
        }
    }
}
