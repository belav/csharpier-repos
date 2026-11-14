//------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Util;

    [System.Runtime.InteropServices.ComVisible(false)]
    internal sealed class NativeMethods
    {
        /*
         * ASPNET_ISAPI.DLL
         */
        private NativeMethods() { }

        [DllImport("Fusion.dll", CharSet = CharSet.Auto)]
        internal static extern int CreateAssemblyCache(
            out IAssemblyCache ppAsmCache,
            uint dwReserved
        );
    }
}
