using System;

public class DeadCode
{
    public static void Main()
    {
        SomeFunc("...");
    }

    public static string SomeFunc(string str)
    {
        return str;
        int i = 0,
            pos = 0;
    }
}
