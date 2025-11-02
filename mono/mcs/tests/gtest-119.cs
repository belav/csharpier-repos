// Compiler options: -unsafe
using System;

public class Tests
{
    public static unsafe void Main()
    {
        Console.WriteLine(typeof(void).Name);
        Console.WriteLine(typeof(void*).Name);
        Console.WriteLine(typeof(void**).Name);
    }
}
