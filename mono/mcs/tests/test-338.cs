class X
{
    public static void Main() { }

    Y Y;

    static object z = Y.Z.I;

    static Y.Z fz()
    {
        return Y.Z.I;
    }
}

public class Y
{
    public class Z
    {
        public static readonly Z I = new Z();
    }
}
