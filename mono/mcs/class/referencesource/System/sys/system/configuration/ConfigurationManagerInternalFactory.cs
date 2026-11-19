//------------------------------------------------------------------------------
// <copyright file="ConfigurationManagerInternalFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration
{
    using System.Configuration.Internal;

    //
    // class ConfigurationManagerInternalFactory manages access to a
    // single instance of ConfigurationManagerInternal.
    //
    internal static class ConfigurationManagerInternalFactory
    {
        private const string ConfigurationManagerInternalTypeString =
            "System.Configuration.Internal.ConfigurationManagerInternal, "
            + AssemblyRef.SystemConfiguration;

        private static volatile IConfigurationManagerInternal s_instance;

        internal static IConfigurationManagerInternal Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (IConfigurationManagerInternal)
                        TypeUtil.CreateInstanceWithReflectionPermission(
                            ConfigurationManagerInternalTypeString
                        );
                }

                return s_instance;
            }
        }
    }
}
