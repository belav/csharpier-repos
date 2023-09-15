//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
#pragma warning disable 1634, 1691
namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Xml;

    class WebFaultClientMessageInspector : IClientMessageInspector
    {
        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply != null)
            {
                HttpResponseMessageProperty prop = (HttpResponseMessageProperty)
                    reply.Properties[HttpResponseMessageProperty.Name];
                if (prop != null && prop.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationException(prop.StatusDescription)
                    );
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}
