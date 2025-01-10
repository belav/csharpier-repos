//------------------------------------------------------------------------------
// <copyright file="EmptyImpersonationContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;

    // An impersonation context that does nothing
    // Used in cases where the Host does not require impersonation.
    class EmptyImpersonationContext : IDisposable
    {
        static volatile IDisposable s_emptyImpersonationContext;

        internal static IDisposable GetStaticInstance()
        {
            if (s_emptyImpersonationContext == null)
            {
                s_emptyImpersonationContext = new EmptyImpersonationContext();
            }

            return s_emptyImpersonationContext;
        }

        public void Dispose() { }
    }
}
