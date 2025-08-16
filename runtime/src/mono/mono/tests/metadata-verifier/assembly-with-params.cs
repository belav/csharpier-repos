using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class SimpleArgs
{
    public static int DefaultParam([DefaultParameterValue(99)] int a, int b)
    {
        return 0;
    }

    [DllImport("foo.so")]
    public static extern void MarshalParam([MarshalAs(UnmanagedType.LPWStr)] string a);
}

public class LastClass
{
    public static void Main() { }
}
