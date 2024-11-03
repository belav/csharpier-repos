//
// fixed
//
class Location
{
    public static int Null
    {
        get { return 1; }
    }
}

class X
{
    Location Location;

    X()
    {
        int a = Location.Null;
    }

    public static void Main() { }
}
