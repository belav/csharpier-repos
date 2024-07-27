using System;

class Foo
{
    public static int i = 0;
}

class Bar
{
    public static int j;

    static Bar()
    {
        j = Foo.i;
    }
}

class Bug
{
    public static int Main()
    {
        Foo.i = 5;
        if (Bar.j != 5)
            return 1;

        return 0;
    }
}
