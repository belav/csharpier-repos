//------------------------------------------------------------------------------
// <copyright file="ClientTargetSection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Web.Util;
    using System.Xml;

    public sealed class ClientTargetSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties;

        #region Property Declarations
        private static readonly ConfigurationProperty _propClientTargets =
            new ConfigurationProperty(
                null,
                typeof(ClientTargetCollection),
                null,
                ConfigurationPropertyOptions.IsRequired
                    | ConfigurationPropertyOptions.IsDefaultCollection
            );
        #endregion

        static ClientTargetSection()
        {
            // Property initialization
            _properties = new ConfigurationPropertyCollection();
            _properties.Add(_propClientTargets);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ClientTargetCollection ClientTargets
        {
            get { return (ClientTargetCollection)base[_propClientTargets]; }
        }
    }
}
