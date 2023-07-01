using System;

namespace A
{
    class CAttribute : Attribute { }
}

namespace B
{
    class CAttribute : Attribute { }
}

namespace Foo
{
    using A;

    using C = A.CAttribute;
    using B;

    [C]
    class Foo
    {
        public static void Main() { }
    }
}
