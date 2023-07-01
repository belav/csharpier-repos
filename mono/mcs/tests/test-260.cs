using x;
using Test = x.Test;
using y;

namespace x
{
    public class Test { }
}

namespace y
{
    public class Test { }
}

namespace b
{
    public class a
    {
        public static void Main()
        {
            // Test should be an alias to x.Test
            Test test = new Test();
        }
    }
}
