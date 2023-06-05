// Compiler options: -langversion:default

namespace Test1
{
    public class Base { }

    partial public class Foo : Base { }

    partial public class Foo : Base { }
}

namespace Test2
{
    public interface Base { }

    partial public class Foo : Base { }

    partial public class Foo : Base { }
}

partial public class ReflectedType { }

partial class ReflectedType { }

partial class D { }

partial public class D { }

partial class D { }

class X
{
    public static void Main() { }
}
