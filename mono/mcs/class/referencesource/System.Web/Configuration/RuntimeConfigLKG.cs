//------------------------------------------------------------------------------
// <copyright file="RuntimeConfigLKG.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Configuration;
using System.Configuration.Internal;
using System.Security.Permissions;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using ClassHostingEnvironment = System.Web.Hosting.HostingEnvironment;
using ClassHttpRuntime = System.Web.HttpRuntime;
using System.Web.Util;

namespace System.Web.Configuration
{
    //
    // RuntimeConfig class that implements LKG functionality.
    //
    internal class RuntimeConfigLKG : RuntimeConfig
    {
        //
        // Note that if configRecord is null, we are the LKG for the ClientRuntimeConfig.
        //
        internal RuntimeConfigLKG(IInternalConfigRecord configRecord)
            : base(configRecord, true) { }

        [ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
        protected override object GetSectionObject(string sectionName)
        {
            if (_configRecord != null)
            {
                return _configRecord.GetLkgSection(sectionName);
            }
            else
            {
                try
                {
                    return ConfigurationManager.GetSection(sectionName);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
