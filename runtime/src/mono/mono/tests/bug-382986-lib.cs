using System;

namespace Repro
{
    public class Base
    {
        protected internal virtual int Test()
        {
            return 1;
        }
    }

    public class Generic<T>
        where T : Base
    {
        public int Run(T t)
        {
            return t.Test();
        }
    }
}
