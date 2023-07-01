using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.RateLimiting;

internal sealed class DefaultKeyTypeEqualityComparer : IEqualityComparer<DefaultKeyType>
{
    public bool Equals(DefaultKeyType x, DefaultKeyType y)
    {
        var xKey = x.Key;
        var yKey = y.Key;
        if (xKey == null && yKey == null)
        {
            return string.Equals(x.PolicyName, y.PolicyName, StringComparison.Ordinal);
        }
        else if (xKey == null || yKey == null)
        {
            return false;
        }

        return string.Equals(x.PolicyName, y.PolicyName, StringComparison.Ordinal)
            && xKey.Equals(yKey);
    }

    public int GetHashCode([DisallowNull] DefaultKeyType obj)
    {
        return HashCode.Combine(obj.Key, obj.PolicyName);
    }
}
