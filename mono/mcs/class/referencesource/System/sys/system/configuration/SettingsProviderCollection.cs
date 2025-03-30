//------------------------------------------------------------------------------
// <copyright file="SettingsProviderCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration
{
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Runtime.Serialization;

    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////
    public class SettingsProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (!(provider is SettingsProvider))
            {
                throw new ArgumentException(
                    SR.GetString(
                        SR.Config_provider_must_implement_type,
                        typeof(SettingsProvider).ToString()
                    ),
                    "provider"
                );
            }

            base.Add(provider);
        }

        public new SettingsProvider this[string name]
        {
            get { return (SettingsProvider)base[name]; }
        }
    }
}
