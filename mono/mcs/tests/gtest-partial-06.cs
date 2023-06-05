partial class Test
{
    partial static void Foo<T>();

    partial static void Baz<T>();

    partial static void Baz<U>() { }

    partial static void Bar<T>(T t)
        where T : class;

    partial static void Bar<U>(U u)
        where U : class { }

    public static void Main()
    {
        Foo<long>();
        Baz<string>();
        Bar<Test>(null);
    }
}
