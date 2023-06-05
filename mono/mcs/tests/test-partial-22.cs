partial class C
{
    partial static void Partial(int i);

    public static int Main()
    {
        Partial(1);
        return 0;
    }
}
