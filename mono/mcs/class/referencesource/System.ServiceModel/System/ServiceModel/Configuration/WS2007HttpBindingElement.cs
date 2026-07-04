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

    public partial class WS2007HttpBindingElement : WSHttpBindingElement
    {
        public WS2007HttpBindingElement(string name)
            : base(name) { }

        public WS2007HttpBindingElement()
            : this(null) { }

        protected override Type BindingElementType
        {
            get { return typeof(WS2007HttpBinding); }
        }
    }
}
