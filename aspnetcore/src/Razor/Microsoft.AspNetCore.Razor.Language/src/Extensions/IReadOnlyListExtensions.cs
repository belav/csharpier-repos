using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Razor.Language.Extensions;

internal static class IReadOnlyListExtensions
{
    public static bool Any<T, TArg>(
        this IReadOnlyList<T> list,
        Func<T, TArg, bool> predicate,
        TArg arg
    )
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        for (var i = 0; i < list.Count; i++)
        {
            if (predicate(list[i], arg))
            {
                return true;
            }
        }

        return false;
    }

    public static bool All<T, TArg>(
        this IReadOnlyList<T> list,
        Func<T, TArg, bool> predicate,
        TArg arg
    )
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        for (var i = 0; i < list.Count; i++)
        {
            if (!predicate(list[i], arg))
            {
                return false;
            }
        }

        return true;
    }
}
