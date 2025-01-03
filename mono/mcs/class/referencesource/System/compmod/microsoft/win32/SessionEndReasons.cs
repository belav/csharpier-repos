//------------------------------------------------------------------------------
// <copyright file="SessionEndReasons.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 */
namespace Microsoft.Win32
{
    using System;
    using System.Diagnostics;

    /// <devdoc>
    ///    <para> Specifies how the current
    ///       logon session is ending.</para>
    /// </devdoc>
    public enum SessionEndReasons
    {
        /// <devdoc>
        ///      The user is logging off.  The system may continue
        ///      running but the user who started this application
        ///      is logging off.
        /// </devdoc>
        Logoff = 1,

        /// <devdoc>
        ///      The system is shutting down.
        /// </devdoc>
        SystemShutdown = 2,
    }
}
