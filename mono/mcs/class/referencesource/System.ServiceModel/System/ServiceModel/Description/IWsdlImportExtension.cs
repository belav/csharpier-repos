//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Description
{
    using System.Collections.Generic;
    using WsdlNS = System.Web.Services.Description;
    using System.Xml;
    using System.Xml.Schema;

    public interface IWsdlImportExtension
    {
        void BeforeImport(
            WsdlNS.ServiceDescriptionCollection wsdlDocuments,
            XmlSchemaSet xmlSchemas,
            ICollection<XmlElement> policy
        );

        void ImportContract(WsdlImporter importer, WsdlContractConversionContext context);
        void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context);
    }
}
