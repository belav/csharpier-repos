//------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 *
 * Copyright (c) 1998-1999, Microsoft Corporation
 *
 */

namespace System.Web.Util
{
    using System.Data.SqlClient;
    using System.Security;
    using System.Security.Permissions;

    static class Permission
    {
        internal static bool HasSqlClientPermission()
        {
            NamedPermissionSet permissionset = HttpRuntime.NamedPermissionSet;

            // If we don't have a NamedPermissionSet, we're in full trust
            if (permissionset == null)
                return true;

            // Check that the user has unrestricted SqlClientPermission
            IPermission allowedPermission = permissionset.GetPermission(
                typeof(SqlClientPermission)
            );

            if (allowedPermission == null)
            {
                return false;
            }

            IPermission askedPermission = null;
            try
            {
                askedPermission = new SqlClientPermission(PermissionState.Unrestricted);
            }
            catch
            {
                return false;
            }

            return askedPermission.IsSubsetOf(allowedPermission);
        }
    }
}
