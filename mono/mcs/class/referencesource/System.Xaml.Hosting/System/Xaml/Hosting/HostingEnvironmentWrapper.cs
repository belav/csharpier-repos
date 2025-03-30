//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.Xaml.Hosting
{
    using System;
    using System.Runtime;
    using System.Security;
    using System.Web;
    using System.Web.Hosting;

    static class HostingEnvironmentWrapper
    {
        public static IDisposable UnsafeImpersonate()
        {
            return HostingEnvironment.Impersonate();
        }
    }
}
