using System;
using C = A.D;

class AA
{
    internal class D : Exception { }
}

class A : AA
{
    public static void Main()
    {
        object o = new C();
    }
}
