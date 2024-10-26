//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel.Channels;

    [ConfigurationCollection(
        typeof(CustomBindingElement),
        AddItemName = ConfigurationStrings.Binding
    )]
    public sealed class CustomBindingElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<CustomBindingElement>
    {
        public CustomBindingElementCollection()
            : base(ConfigurationStrings.Binding) { }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }

            CustomBindingElement configElementKey = (CustomBindingElement)element;
            return configElementKey.Name;
        }
    }
}
