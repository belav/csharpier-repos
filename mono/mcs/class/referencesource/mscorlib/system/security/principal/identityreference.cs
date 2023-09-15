// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
// <OWNER>Microsoft</OWNER>
//

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Security.Policy; // defines Url class

namespace System.Security.Principal
{
    [System.Runtime.InteropServices.ComVisible(false)]
    public abstract class IdentityReference
    {
        internal IdentityReference()
        {
            // exists to prevent creation user-derived classes (for now)
        }

        //      public abstract string Scheme { get; }

        public abstract string Value { get; }

        //      public virtual Url Url
        //      {
        //          get { return new Url(""); } //


        public abstract bool IsValidTargetType(Type targetType);

        public abstract IdentityReference Translate(Type targetType);

        public abstract override bool Equals(object o);

        public abstract override int GetHashCode();

        public abstract override string ToString();

        public static bool operator ==(IdentityReference left, IdentityReference right)
        {
            object l = left;
            object r = right;

            if (l == null && r == null)
            {
                return true;
            }
            else if (l == null || r == null)
            {
                return false;
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(IdentityReference left, IdentityReference right)
        {
            return !(left == right); // invoke operator==
        }
    }
}
