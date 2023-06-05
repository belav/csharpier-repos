//
// Thisis just a compilation test for bug 61593
using System;

namespace AppFramework.Util
{
    public class Logic
    {
        public static bool EnumInSet(Enum anEnum, Enum[] checkSet)
        {
            foreach (Enum aVal in checkSet)
            {
                if (aVal == anEnum)
                {
                    return true;
                }
            }
            return false;
        }

        public static void Main() { }
    }
}
