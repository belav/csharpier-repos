//------------------------------------------------------------------------------
// <copyright file="RpcImpers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Configuration
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Configuration.Internal;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Util;
    using System.Xml;

    internal enum RpcImpers
    { // RPC_C_IMP_LEVEL_xxx
        Default = 0,
        Anonymous = 1,
        Identify = 2,
        Impersonate = 3,
        Delegate = 4,
    }
}
