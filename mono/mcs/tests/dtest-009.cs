partial class B
{
    partial static void Test(int a);

    partial static void Test(int x) { }

    public static int Main()
    {
        Test(a: 5);

        dynamic d = -1;
        Test(a: d);

        return 0;
    }
}
