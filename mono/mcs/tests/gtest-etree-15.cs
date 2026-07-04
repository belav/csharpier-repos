using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public static class Foo
{
    public static int Helper(Expression<Predicate<int>> match)
    {
        return 0;
    }

    public static void Main()
    {
        Expression<Action<List<int>>> exp = x => x.Add(Helper(i => true));
        exp.Compile()(new List<int> { 1 });
    }
}
