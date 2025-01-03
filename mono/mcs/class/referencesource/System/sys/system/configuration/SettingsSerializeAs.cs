//------------------------------------------------------------------------------
// <copyright file="SettingsSerializeAs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration.Provider;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    public enum SettingsSerializeAs
    {
        String = 0,
        Xml = 1,
        Binary = 2,
        ProviderSpecific = 3,
    }
}
