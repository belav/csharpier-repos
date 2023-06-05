// This is used to debug an ordering dependent bug.  The counterpart is test-388.cs.
//

using System;
using System.Collections;
using System.Reflection;

namespace Schemas
{
    partial public class basefieldtype
    {
        public virtual object Instantiate()
        {
            return null;
        }
    }

    partial public class fieldtype
    {
        public override object Instantiate()
        {
            Console.WriteLine("Instantiating type '{0}'", id);
            return null;
        }
    }

    partial public class compoundfield
    {
        public override object Instantiate()
        {
            Console.WriteLine("Instantiating compound field '{0}'", id);
            return null;
        }
    }

    partial public class field
    {
        public object Instantiate()
        {
            Console.WriteLine("Instantiating field '{0}'", id);
            return null;
        }
    }

    partial public class formdata
    {
        public object Instantiate()
        {
            Console.WriteLine("Instantiating form window");
            return null;
        }
    }
}
