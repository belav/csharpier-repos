//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace System.ServiceModel.Routing.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime;
    using System.ServiceModel.Configuration;

    public class SoapProcessingExtensionElement : BehaviorExtensionElement
    {
        [SuppressMessage(
            FxCop.Category.Configuration,
            FxCop.Rule.ConfigurationPropertyAttributeRule,
            Justification = "this is not a configuration property"
        )]
        public override Type BehaviorType
        {
            get { return typeof(SoapProcessingBehavior); }
        }

        protected internal override object CreateBehavior()
        {
            SoapProcessingBehavior behavior = new SoapProcessingBehavior();
            behavior.ProcessMessages = this.ProcessMessages;
            return behavior;
        }

        [ConfigurationProperty(
            ConfigurationStrings.ProcessMessages,
            DefaultValue = true,
            Options = ConfigurationPropertyOptions.None
        )]
        public bool ProcessMessages
        {
            get { return (bool)this[ConfigurationStrings.ProcessMessages]; }
            set { this[ConfigurationStrings.ProcessMessages] = value; }
        }
    }
}
