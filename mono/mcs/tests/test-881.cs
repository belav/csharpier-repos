using System;

namespace A
{
    class XAttribute : Attribute { }
}

namespace B
{
    class XAttribute : Attribute { }
}

namespace C
{
    using A;
    using X = A.XAttribute;
    using B;

    [X]
    class Test
    {
        public static void Main() { }
    }
}
