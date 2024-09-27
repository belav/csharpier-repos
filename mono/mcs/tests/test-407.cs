// Compiler options: -unsafe

struct Obsolete
{
    int a;
}

struct A
{
    int a,
        b;
}

class MainClass
{
    public static unsafe void Main()
    {
        System.Console.WriteLine(sizeof(Obsolete));
    }
}
