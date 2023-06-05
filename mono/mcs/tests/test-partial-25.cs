using System;

partial class C
{
    partial static void Partial(int i = 8);

    partial static void Partial(int i)
    {
        if (i != 8)
            throw new ApplicationException();
    }

    public static int Main()
    {
        Partial();
        return 0;
    }
}
