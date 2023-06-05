using System;

namespace Bug
{
    partial public static class GL
    {
        partial static class Core
        {
            internal static bool A()
            {
                return true;
            }
        }

        /*internal static partial class Bar
        {
            internal static bool A () { return true; }
        }*/
    }

    partial class GL
    {
        public static void Main()
        {
            Core.A();
            //Bar.A ();
        }

        partial internal class Core { }

        /*partial class Bar
        {
        }*/
    }
}
