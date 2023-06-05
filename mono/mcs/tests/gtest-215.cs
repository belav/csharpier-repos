public class R { }

public class A<T>
    where T : R { }

partial public class D : A<R> { }

class MainClass
{
    public static void Main() { }
}
