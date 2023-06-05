partial class C
{
    public static void Foo(this int t) { }
}

partial static class C
{
    public static void Foo() { }
}

class Test
{
    public static void Main()
    {
        1.Foo();
    }
}
