namespace n1
{
    class Attribute { }
}

namespace n3
{
    using System;
    using n1;

    class A
    {
        void Attribute() { }

        void X()
        {
            Attribute();
        }

        public static void Main()
        {
            new A().X();
        }
    }
}
