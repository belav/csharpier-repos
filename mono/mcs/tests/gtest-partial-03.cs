using System.Collections.Generic;

class Variable { }

partial internal class Test<T> { }

partial internal class Test<T>
    where T : IList<Variable>
{
    public Test(T t)
    {
        var val = t.Count;
    }
}

partial internal class Test<T> { }

class CC
{
    public static void Main()
    {
        new Test<List<Variable>>(new List<Variable>());
    }
}
