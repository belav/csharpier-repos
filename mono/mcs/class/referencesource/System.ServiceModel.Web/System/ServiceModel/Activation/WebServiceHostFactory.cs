//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Activation
{
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Web;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Hosting;

    public class WebServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebServiceHost(serviceType, baseAddresses);
        }
    }
}
