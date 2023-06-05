using System;

partial class C
{
    partial static void Partial(int i);

    partial static void Partial(string i);

    public static int Main()
    {
        Partial(1);
        Partial("x");
        return 0;
    }
}
