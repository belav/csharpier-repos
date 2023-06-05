// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace ILLink.Shared.TypeSystemProxy
{
    partial internal readonly struct GenericParameterProxy
    {
        public GenericParameterProxy(ITypeParameterSymbol typeParameterSymbol) =>
            TypeParameterSymbol = typeParameterSymbol;

        partial internal bool HasDefaultConstructorConstraint() =>
            TypeParameterSymbol.HasConstructorConstraint
            | TypeParameterSymbol.HasValueTypeConstraint
            | TypeParameterSymbol.HasUnmanagedTypeConstraint;

        public readonly ITypeParameterSymbol TypeParameterSymbol;

        public override string ToString() => TypeParameterSymbol.ToString();
    }
}
