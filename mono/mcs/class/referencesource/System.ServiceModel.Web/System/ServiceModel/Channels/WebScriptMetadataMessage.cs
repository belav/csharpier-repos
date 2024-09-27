//----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------------------

#pragma warning disable 1634 // Stops compiler from warning about unknown warnings (for Presharp)

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml;
    using DiagnosticUtility = System.ServiceModel.DiagnosticUtility;

    class WebScriptMetadataMessage : BodyWriterMessage
    {
        const string proxyContentTag = "JavaScriptProxy";
        string proxyContent;

        public WebScriptMetadataMessage(string action, string proxyContent)
            : base(MessageVersion.None, action, new WebScriptMetadataBodyWriter(proxyContent))
        {
            this.proxyContent = proxyContent;
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(proxyContentTag);
            writer.WriteCData(proxyContent);
            writer.WriteEndElement();
        }

        class WebScriptMetadataBodyWriter : BodyWriter
        {
            string proxyContent;

            public WebScriptMetadataBodyWriter(string proxyContent)
                : base(true)
            {
                this.proxyContent = proxyContent;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteRaw(proxyContent);
            }
        }
    }
}
