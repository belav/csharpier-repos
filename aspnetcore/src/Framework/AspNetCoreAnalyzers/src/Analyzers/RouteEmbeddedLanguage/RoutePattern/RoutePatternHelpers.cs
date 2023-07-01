using System.Collections.Immutable;
using Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.Infrastructure.EmbeddedSyntax;
using Microsoft.CodeAnalysis.EmbeddedLanguages.VirtualChars;

namespace Microsoft.AspNetCore.Analyzers.RouteEmbeddedLanguage.RoutePattern;

using RoutePatternToken = EmbeddedSyntaxToken<RoutePatternKind>;

internal static class RoutePatternHelpers
{
    public static RoutePatternToken CreateToken(
        RoutePatternKind kind,
        VirtualCharSequence virtualChars
    ) => new(kind, virtualChars, ImmutableArray<EmbeddedDiagnostic>.Empty, value: null);

    public static RoutePatternToken CreateMissingToken(RoutePatternKind kind) =>
        CreateToken(kind, VirtualCharSequence.Empty);
}
