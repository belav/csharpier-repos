// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using ILLink.RoslynAnalyzer;
using Microsoft.CodeAnalysis;

namespace ILLink.Shared.TypeSystemProxy
{
    partial readonly struct MethodProxy
    {
        public MethodProxy(IMethodSymbol method) => Method = method;

        public readonly IMethodSymbol Method;

        public string Name
        {
            get => Method.Name;
        }

        public string GetDisplayName() => Method.GetDisplayName();

        partial internal bool IsDeclaredOnType(string fullTypeName) =>
            IsTypeOf(Method.ContainingType, fullTypeName);

        partial internal bool HasMetadataParameters() => Method.Parameters.Length > 0;

        partial internal int GetMetadataParametersCount() => Method.GetMetadataParametersCount();

        partial internal int GetParametersCount() => Method.GetParametersCount();

        partial internal ParameterProxyEnumerable GetParameters() => Method.GetParameters();

        partial internal ParameterProxy GetParameter(ParameterIndex index) =>
            Method.GetParameter(index);

        partial internal bool HasGenericParameters() => Method.IsGenericMethod;

        partial internal bool HasGenericParametersCount(int genericParameterCount) =>
            Method.TypeParameters.Length == genericParameterCount;

        partial internal ImmutableArray<GenericParameterProxy> GetGenericParameters()
        {
            if (Method.TypeParameters.IsEmpty)
                return ImmutableArray<GenericParameterProxy>.Empty;

            var builder = ImmutableArray.CreateBuilder<GenericParameterProxy>(
                Method.TypeParameters.Length
            );
            foreach (var typeParameter in Method.TypeParameters)
            {
                builder.Add(new GenericParameterProxy(typeParameter));
            }

            return builder.ToImmutableArray();
        }

        partial internal bool IsStatic() => Method.IsStatic;

        partial internal bool HasImplicitThis() => Method.HasImplicitThis();

        partial internal bool ReturnsVoid() =>
            Method.ReturnType.SpecialType == SpecialType.System_Void;

        static bool IsTypeOf(ITypeSymbol type, string fullTypeName)
        {
            if (type is not INamedTypeSymbol namedType)
                return false;

            return namedType.HasName(fullTypeName);
        }

        public override string ToString() => Method.ToString();
    }
}
