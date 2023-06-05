// Compiler options: support-xml-067.cs -doc:xml-067.xml -warnaserror

// Partial types can have documentation on one part only

using System;

namespace Testing
{
    partial
    /// <summary>
    /// description for class Test
    /// </summary>
    public class Test
    {
        /// test
        public Test() { }
    }

    partial public class Test
    {
        /// test 2
        public void Foo() { }

        static void Main() { }
    }
}
