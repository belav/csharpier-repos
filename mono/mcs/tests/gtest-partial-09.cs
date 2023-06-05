namespace A
{
    partial public class B<T>
    {
        partial public class C
        {
            public class A { }
        }
    }
}

namespace A
{
    partial public abstract class B<T>
        where T : B<T>.C { }
}

namespace A
{
    partial public class B<T>
    {
        partial public class C : I { }
    }
}

namespace A
{
    public interface Ibase { }

    partial public class B<T>
    {
        public interface I : Ibase { }
    }
}

namespace A
{
    class Bar : B<Bar>.C { }

    public class Test
    {
        public static void Main()
        {
            Ibase b = new Bar();
            System.Console.WriteLine(b != null);
        }
    }
}
