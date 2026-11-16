//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    class WebScriptServiceHost : ServiceHost
    {
        static readonly string WebScriptEndpointKind = "webScriptEndpoint";

        public WebScriptServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses) { }

        protected override void OnOpening()
        {
            WebServiceHost.AddAutomaticWebHttpBindingEndpoints(
                this,
                this.ImplementedContracts,
                SR2.GetString(
                    SR2.JsonWebScriptServiceHostOneServiceContract,
                    this.ImplementedContracts.Count
                ),
                SR2.GetString(SR2.JsonWebScriptServiceHostAtLeastOneServiceContract),
                WebScriptEndpointKind
            );
            foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
            {
                if (
                    endpoint.Binding != null
                    && endpoint
                        .Binding.CreateBindingElements()
                        .Find<WebMessageEncodingBindingElement>() != null
                )
                {
                    if (endpoint.Behaviors.Find<WebHttpBehavior>() == null)
                    {
                        ConfigLoader.LoadDefaultEndpointBehaviors(endpoint);
                        if (endpoint.Behaviors.Find<WebHttpBehavior>() == null)
                        {
                            endpoint.Behaviors.Add(new WebScriptEnablingBehavior());
                        }
                    }
                }
            }

            base.OnOpening();
        }
    }
}
