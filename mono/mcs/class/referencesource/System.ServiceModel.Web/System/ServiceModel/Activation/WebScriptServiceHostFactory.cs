//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Activation
{
    using System.IO;
    using System.Reflection;
    using System.ServiceModel.Diagnostics;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Hosting;

    public class WebScriptServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WebScriptServiceHost(serviceType, baseAddresses);
        }
    }
}
