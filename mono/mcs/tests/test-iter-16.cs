using System.Collections;

class Foo
{
    public static IEnumerable foo()
    {
        try
        {
            yield break;
        }
        catch { }
        finally { }
    }

    public static int Main()
    {
        int i = 0;
        foreach (object o in foo())
            ++i;
        return i;
    }
}
