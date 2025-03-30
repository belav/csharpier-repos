//------------------------------------------------------------------------------
// <copyright file="ServerType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Services.Protocols
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Web.Services.Description;
    using System.Xml.Serialization;

    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class ServerType
    {
        Type type;

        public ServerType(Type type)
        {
            this.type = type;
        }

        internal Type Type
        {
            get { return type; }
        }

        internal Evidence Evidence
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
                return Type.Assembly.Evidence;
            }
        }
    }
}
