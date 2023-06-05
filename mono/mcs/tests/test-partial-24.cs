namespace N
{
    using S = System;

    partial public class C
    {
        [S.Obsolete("A")]
        partial void Foo();

        public static void Main() { }
    }
}

namespace N
{
    partial public class C
    {
        partial void Foo() { }
    }
}
