using System.Collections.Generic;
using System.Linq;

class MM
{
    public IEnumerable<int> myEnumerable { get; set; }
}

class Test
{
    public static void Main()
    {
        MM myobject = null;
        (myobject?.myEnumerable?.Any()).GetValueOrDefault(false);
    }
}
