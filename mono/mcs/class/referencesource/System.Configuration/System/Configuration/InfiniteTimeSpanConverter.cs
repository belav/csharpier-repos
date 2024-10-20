//------------------------------------------------------------------------------
// <copyright file="InfiniteTimeSpanConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace System.Configuration
{
    public sealed class InfiniteTimeSpanConverter : ConfigurationConverterBase
    {
        static readonly TypeConverter s_TimeSpanConverter = TypeDescriptor.GetConverter(
            typeof(TimeSpan)
        );

        public override object ConvertTo(
            ITypeDescriptorContext ctx,
            CultureInfo ci,
            object value,
            Type type
        )
        {
            ValidateType(value, typeof(TimeSpan));

            if ((TimeSpan)value == TimeSpan.MaxValue)
            {
                return "Infinite";
            }
            else
            {
                return s_TimeSpanConverter.ConvertToInvariantString(value);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            Debug.Assert(data is string, "data is string");

            if ((string)data == "Infinite")
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                return s_TimeSpanConverter.ConvertFromInvariantString((string)data);
            }
        }
    }
}
