partial
// Compiler options: -langversion:default

public class Test
{
    public readonly Foo TheFoo;

    public Test()
    {
        this.TheFoo = new Foo();
    }

    partial public interface IFoo
    {
        int Hello(Test foo);
    }

    public int TestFoo()
    {
        return TheFoo.Hello(this);
    }
}

partial public class Test
{
    partial public class Foo : IFoo
    {
        int IFoo.Hello(Test test)
        {
            return 2;
        }

        public int Hello(Test test)
        {
            return 1;
        }
    }

    public int TestIFoo(IFoo foo)
    {
        return foo.Hello(this);
    }
}

class X
{
    public static int Main()
    {
        Test test = new Test();
        if (test.TestFoo() != 1)
            return 1;
        if (test.TestIFoo(test.TheFoo) != 2)
            return 2;
        return 0;
    }
}
