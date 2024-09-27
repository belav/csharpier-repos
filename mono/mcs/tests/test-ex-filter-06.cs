using System;
using System.Collections.Generic;
using System.IO;

class C
{
    static int Test<T>()
        where T : Exception
    {
        try
        {
            throw null;
        }
        catch (T t) when (t.Message != null)
        {
            return 0;
        }
    }

    static int Main()
    {
        try
        {
            Test<ApplicationException>();
            return 1;
        }
        catch { }

        if (Test<NullReferenceException>() != 0)
            return 2;

        return 0;
    }
}
