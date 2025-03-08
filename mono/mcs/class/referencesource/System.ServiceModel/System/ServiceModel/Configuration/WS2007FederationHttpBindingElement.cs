//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.ServiceModel.Configuration
{
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Text;

    public partial class WS2007FederationHttpBindingElement : WSFederationHttpBindingElement
    {
        public WS2007FederationHttpBindingElement(string name)
            : base(name) { }

        public WS2007FederationHttpBindingElement()
            : this(null) { }

        protected override Type BindingElementType
        {
            get { return typeof(WS2007FederationHttpBinding); }
        }
    }
}
