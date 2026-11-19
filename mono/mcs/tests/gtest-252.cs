using System;

public static class EqualityComparer<T>
{
    static readonly Type sequencedequalityComparer = typeof(SequencedEqualityComparer<,>);

    public static void Test() { }
}

public class SequencedEqualityComparer<T, W> { }

class X
{
    public static void Main()
    {
        EqualityComparer<int>.Test();
    }
}
