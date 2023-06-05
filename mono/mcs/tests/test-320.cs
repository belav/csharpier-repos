// Compiler options: -unsafe


using System;

unsafe class X
{
    public unsafe X(sbyte* value, int startIndex, int length) { }

    public static void Main()
    {
        new X((sbyte*)null, 0, 10);
    }
}
