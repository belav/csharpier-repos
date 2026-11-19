using System;

public struct Test
{
    int a1;
    int a2;

    public static int Main()
    {
        Test[] tarray = new Test[20];
        tarray[0].a1 = 1;
        tarray[0].a2 = 2;
        if (tarray[0].a1 + tarray[0].a2 != 3)
            return 1;
        return 0;
    }
}
