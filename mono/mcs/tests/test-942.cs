using System;
using System.Diagnostics;
using System.Reflection;

namespace ConditionalAttributeTesting
{
    class MainClass
    {
        public static int Main()
        {
            return HelloWorld();
        }

        [Some("Test")]
        public static int HelloWorld()
        {
            var methodInfo = MethodBase.GetCurrentMethod();
            SomeAttribute someAttribute =
                Attribute.GetCustomAttribute(methodInfo, typeof(SomeAttribute)) as SomeAttribute;
            if (someAttribute != null)
            {
                return 1;
            }

            return 0;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    [Conditional("NOT_DEFINED")]
    public abstract class BaseAttribute : Attribute { }

    public class SomeAttribute : BaseAttribute
    {
        public SomeAttribute(string someText) { }
    }
}
