// Compiler options: -unsafe

using System;

unsafe class Program
{
    public static unsafe int Main()
    {
        float* to = stackalloc float[2];
        to[0] = to[1] = float.MaxValue;
        if (to[0] != float.MaxValue)
            return 1;

        if (to[1] != float.MaxValue)
            return 2;

        return 0;
    }
}
