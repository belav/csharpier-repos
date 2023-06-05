// Compiler options: -unsafe

public class aClass
{
    public unsafe struct foo_t
    {
        fixed public char b[16];
    }

    public static unsafe void Main(string[] args)
    {
        foo_t bar;
        char* oo = bar.b;
    }
}
