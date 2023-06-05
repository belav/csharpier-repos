// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ILCompiler;
using ILCompiler.Dataflow;
using Internal.TypeSystem;
using Internal.TypeSystem.Ecma;

#nullable enable

namespace ILLink.Shared.TypeSystemProxy
{
    partial internal struct ParameterProxy
    {
        partial public ReferenceKind GetReferenceKind() =>
            Method.Method.ParameterReferenceKind((int)Index);

        public TypeDesc ParameterType =>
            IsImplicitThis ? Method.Method.OwningType : Method.Method.Signature[MetadataIndex];

        partial public string GetDisplayName() =>
            IsImplicitThis
                ? "this"
                : (Method.Method is EcmaMethod ecmaMethod)
                    ? ecmaMethod.GetParameterDisplayName(MetadataIndex)
                    : $"#{Index}";

        partial public bool IsTypeOf(string typeName) => ParameterType.IsTypeOf(typeName);

        public bool IsTypeOf(WellKnownType type) => ParameterType.IsTypeOf(type);
    }
}
