using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

using ConfigurationType = System.Configuration.Configuration;

namespace System.ServiceModel.Configuration
{
    partial
    // ChannelEndpointElementCollection
    public sealed class ChannelEndpointElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<ChannelEndpointElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            ChannelEndpointElement el = (ChannelEndpointElement)element;
            return el.Name + ";" + el.Contract;
        }
    }

    partial
    // ClaimTypeElementCollection
    public sealed class ClaimTypeElementCollection
        : ServiceModelConfigurationElementCollection<ClaimTypeElement>,
            ICollection,
            IEnumerable
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClaimTypeElement)element).ClaimType;
        }
    }

    partial
    // ComContractElementCollection
    public sealed class ComContractElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<ComContractElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ComContractElement)element).Name;
        }
    }

    partial
    // ComMethodElementCollection
    public sealed class ComMethodElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<ComMethodElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ComMethodElement)element).ExposedMethod;
        }
    }

    partial
    // ComPersistableTypeElementCollection
    public sealed class ComPersistableTypeElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<ComPersistableTypeElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            // FIXME: ID? anyways, cosmetic COM stuff...
            return ((ComPersistableTypeElement)element).Name;
        }
    }

    partial
    // ComUdtElementCollection
    public sealed class ComUdtElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<ComUdtElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            // FIXME: another property? anyways COM stuff...
            return ((ComUdtElement)element).Name;
        }
    }

    partial
    // CustomBindingElementCollection
    public sealed class CustomBindingElementCollection
        : ServiceModelEnhancedConfigurationElementCollection<CustomBindingElement>,
            ICollection,
            IEnumerable
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CustomBindingElement)element).Name;
        }
    }

    partial
    // IssuedTokenClientBehaviorsElementCollection
    public sealed class IssuedTokenClientBehaviorsElementCollection
        : ServiceModelConfigurationElementCollection<IssuedTokenClientBehaviorsElement>
    {
        [MonoTODO]
        protected override object GetElementKey(ConfigurationElement element)
        {
            throw new NotImplementedException();
        }
    }

    partial
    // StandardBindingElementCollection
    public sealed class StandardBindingElementCollection<TBindingConfiguration>
        : ServiceModelEnhancedConfigurationElementCollection<TBindingConfiguration>,
            ICollection,
            IEnumerable
        where TBindingConfiguration : StandardBindingElement, new()
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StandardBindingElement)element).Name;
        }
    }

    partial
    // TransportConfigurationTypeElementCollection
    public sealed class TransportConfigurationTypeElementCollection
        : ServiceModelConfigurationElementCollection<TransportConfigurationTypeElement>
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TransportConfigurationTypeElement)element).Name;
        }
    }

    partial
    // XPathMessageFilterElementCollection
    public sealed class XPathMessageFilterElementCollection
        : ServiceModelConfigurationElementCollection<XPathMessageFilterElement>
    {
        [MonoTODO]
        protected override object GetElementKey(ConfigurationElement element)
        {
            throw new NotImplementedException();
        }
    }
}
