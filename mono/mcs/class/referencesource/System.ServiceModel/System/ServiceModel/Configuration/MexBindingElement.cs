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

    public abstract class MexBindingElement<TStandardBinding> : StandardBindingElement
        where TStandardBinding : Binding
    {
        protected MexBindingElement(string name)
            : base(name) { }

        protected override Type BindingElementType
        {
            get { return typeof(TStandardBinding); }
        }

        protected override void OnApplyConfiguration(Binding binding) { }
    }
}
