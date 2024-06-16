//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Runtime.Serialization.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;

    internal static class ConfigurationStrings
    {
        static string GetSectionPath(string sectionName)
        {
            return string.Concat(ConfigurationStrings.SectionGroupName, "/", sectionName);
        }

        internal static string DataContractSerializerSectionPath
        {
            get
            {
                return ConfigurationStrings.GetSectionPath(
                    ConfigurationStrings.DataContractSerializerSectionName
                );
            }
        }

        internal static string NetDataContractSerializerSectionPath
        {
            get
            {
                return ConfigurationStrings.GetSectionPath(
                    ConfigurationStrings.NetDataContractSerializerSectionName
                );
            }
        }

        internal const string SectionGroupName = "system.runtime.serialization";

        internal const string DefaultCollectionName = ""; // String.Empty
        internal const string DeclaredTypes = "declaredTypes";
        internal const string Index = "index";
        internal const string Parameter = "parameter";
        internal const string Type = "type";
        internal const string EnableUnsafeTypeForwarding = "enableUnsafeTypeForwarding";
        internal const string DataContractSerializerSectionName = "dataContractSerializer";
        internal const string NetDataContractSerializerSectionName = "netDataContractSerializer";
    }
}
