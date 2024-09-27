//------------------------------------------------------------------------------
// <copyright file="IPartialSessionState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * IPartialSessionState
 *
 * Copyright (c) 1998-1999, Microsoft Corporation
 *
 */

namespace System.Web.SessionState
{
    using System.Collections.Generic;
    using System.Security.Permissions;

    /*
     * Marker interface to indicate that class uses granular session state.
     */


    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public interface IPartialSessionState
    {
        IList<string> PartialSessionStateKeys { get; }
    }
}
