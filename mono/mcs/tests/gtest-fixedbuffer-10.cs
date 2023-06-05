// Compiler options: -unsafe

using System;

public class Program
{
    public static void Main()
    {
        new TestStruct("a");
    }
}

public unsafe struct TestStruct
{
    fixed private byte symbol[30];

    public TestStruct(string a) { }

    public static TestStruct Default
    {
        get
        {
            TestStruct h;
            return h;
        }
    }
}
