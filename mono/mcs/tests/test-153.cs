#define DEBUG
using System;
using System.Diagnostics;
using System.Text;

class Z
{
    public static void Test2(string message, params object[] args) { }

    public static void Test(string message, params object[] args)
    {
        Test2(message, args);
    }

    public static int Main()
    {
        Test("TEST");
        Test("Foo", 8);
        Test("Foo", 8, 9, "Hello");
        return 0;
    }
}
