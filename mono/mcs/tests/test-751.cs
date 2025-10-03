using LocalNS;
using OneMoreNS;
using SomeOtherNS;

namespace SomeOtherNS.Compiler { }

namespace OneMoreNS.Compiler { }

namespace LocalNS
{
    public class Compiler { }
}

namespace System.Local
{
    class M
    {
        public static void Main()
        {
            Compiler c = new LocalNS.Compiler();
        }
    }
}
