// Compiler options: -warnaserror

using System;

[assembly: CLSCompliant(true)]

public class CLSClass
{
    public byte XX
    {
        get { return 5; }
    }

    public static void Main() { }
}

public class Big
{
    [CLSCompliant(false)]
    public static implicit operator Big(uint value)
    {
        return null;
    }
}

[CLSCompliant(false)]
partial public class C1 { }

partial public class C1
{
    public void method(uint u) { }
}
