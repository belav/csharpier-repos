using System;

partial static class C
{
    partial static void Foo_1(this string s);

    [Obsolete]
    partial static void Foo_2(string s);

    public static void Main() { }
}

partial class D
{
    partial static void Method(this int a);
}

partial static class D
{
    partial static void Method(this int a) { }
}
