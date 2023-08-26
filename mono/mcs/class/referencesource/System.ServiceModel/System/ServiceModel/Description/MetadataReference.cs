//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using WsdlNS = System.Web.Services.Description;

    [XmlRoot(
        ElementName = MetadataStrings.MetadataExchangeStrings.MetadataReference,
        Namespace = MetadataStrings.MetadataExchangeStrings.Namespace
    )]
    public class MetadataReference : IXmlSerializable
    {
        EndpointAddress address;
        AddressingVersion addressVersion;
        Collection<XmlAttribute> attributes = new Collection<XmlAttribute>();
        static XmlDocument Document = new XmlDocument();

        public MetadataReference() { }

        public MetadataReference(EndpointAddress address, AddressingVersion addressVersion)
        {
            this.address = address;
            this.addressVersion = addressVersion;
        }

        public EndpointAddress Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        public AddressingVersion AddressVersion
        {
            get { return this.addressVersion; }
            set { this.addressVersion = value; }
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.address = EndpointAddress.ReadFrom(
                XmlDictionaryReader.CreateDictionaryReader(reader),
                out this.addressVersion
            );
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (address != null)
            {
                address.WriteContentsTo(this.addressVersion, writer);
            }
        }
    }
}
