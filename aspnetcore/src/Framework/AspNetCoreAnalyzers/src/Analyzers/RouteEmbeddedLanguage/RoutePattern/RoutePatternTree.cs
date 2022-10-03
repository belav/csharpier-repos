// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.Infrastructure.EmbeddedSyntax;
using Microsoft.CodeAnalysis.EmbeddedLanguages.VirtualChars;

namespace Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.RoutePattern;

internal sealed class RoutePatternTree : EmbeddedSyntaxTree<RoutePatternKind, RoutePatternNode, RoutePatternCompilationUnit>
{
    public readonly ImmutableDictionary<string, RouteParameter> RouteParameters;

    public RoutePatternTree(
        VirtualCharSequence text,
        RoutePatternCompilationUnit root,
        ImmutableArray<EmbeddedDiagnostic> diagnostics,
        ImmutableDictionary<string, RouteParameter> routeParameters)
        : base(text, root, diagnostics)
    {
        RouteParameters = routeParameters;
    }
}

internal readonly struct RouteParameter
{
    public RouteParameter(string name, bool encodeSlashes, string defaultValue, bool isOptional, bool isCatchAll, ImmutableArray<string> policies)
    {
        Name = name;
        EncodeSlashes = encodeSlashes;
        DefaultValue = defaultValue;
        IsOptional = isOptional;
        IsCatchAll = isCatchAll;
        Policies = policies;
    }

    public readonly string Name;
    public readonly bool EncodeSlashes;
    public readonly string DefaultValue;
    public readonly bool IsOptional;
    public readonly bool IsCatchAll;
    public readonly ImmutableArray<string> Policies;

    public override string ToString()
    {
        return Name;
    }
}
