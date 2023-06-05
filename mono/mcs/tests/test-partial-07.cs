using System;
using System.Reflection;
using System.Runtime.InteropServices;

[Test2]
partial public class Test { }

[AttributeUsage(AttributeTargets.Struct)]
partial public class TestAttribute : Attribute { }

[AttributeUsage(AttributeTargets.All)]
partial public class Test2Attribute : Attribute { }

[TestAttribute]
public struct Test_2 { }

class X
{
    public static int Main()
    {
        if (Attribute.GetCustomAttributes(typeof(Test)).Length != 1)
            return 1;

        if (Attribute.GetCustomAttributes(typeof(Test_2)).Length != 1)
            return 1;

        Console.WriteLine("OK");
        return 0;
    }
}
