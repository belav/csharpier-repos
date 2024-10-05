//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Activities.XamlIntegration
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml.Linq;

    [MarkupExtensionReturnType(typeof(object))]
    public sealed class PropertyReferenceExtension<T> : MarkupExtension
    {
        public PropertyReferenceExtension()
            : base() { }

        public string PropertyName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(this.PropertyName))
            {
                object targetObject = ActivityWithResultConverter.GetRootTemplatedActivity(
                    serviceProvider
                );
                if (targetObject != null)
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(targetObject)[
                        PropertyName
                    ];

                    if (property != null)
                    {
                        return property.GetValue(targetObject);
                    }
                }
            }

            throw FxTrace.Exception.AsError(
                new InvalidOperationException(SR.PropertyReferenceNotFound(this.PropertyName))
            );
        }
    }
}
