using System;

namespace Microsoft.AspNetCore.Razor.Language.Legacy;

internal class ExpressionChunkGenerator : ISpanChunkGenerator
{
    private static readonly int TypeHashCode = typeof(ExpressionChunkGenerator).GetHashCode();

    public override string ToString()
    {
        return "Expr";
    }

    public override bool Equals(object obj)
    {
        return obj != null && GetType() == obj.GetType();
    }

    public override int GetHashCode()
    {
        return TypeHashCode;
    }
}
