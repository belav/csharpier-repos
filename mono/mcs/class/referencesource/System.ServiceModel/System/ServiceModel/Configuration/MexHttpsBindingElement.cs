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

    public partial class MexHttpsBindingElement : MexBindingElement<WSHttpBinding>
    {
        public MexHttpsBindingElement(string name)
            : base(name) { }

        public MexHttpsBindingElement()
            : this(null) { }
    }
}
