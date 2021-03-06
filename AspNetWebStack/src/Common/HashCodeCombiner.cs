// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;

namespace Microsoft.Internal.Web.Utils
{
    internal class HashCodeCombiner
    {
        private long _combinedHash64 = 0x1505L;

        public int CombinedHash
        {
            get { return _combinedHash64.GetHashCode(); }
        }

        public HashCodeCombiner Add(IEnumerable e)
        {
            if (e == null)
            {
                Add(0);
            }
            else
            {
                int count = 0;
                foreach (object o in e)
                {
                    Add(o);
                    count++;
                }
                Add(count);
            }
            return this;
        }

        public HashCodeCombiner Add(int i)
        {
            _combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
            return this;
        }

        public HashCodeCombiner Add(object o)
        {
            int hashCode = (o != null) ? o.GetHashCode() : 0;
            Add(hashCode);
            return this;
        }

        public static HashCodeCombiner Start()
        {
            return new HashCodeCombiner();
        }
    }
}
