//------------------------------------------------------------------------------
// <copyright file="TimeSpanSecondsOrInfiniteConverter.cs" company="Microsoft">
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
    public sealed class TimeSpanSecondsOrInfiniteConverter : TimeSpanSecondsConverter
    {
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
                return base.ConvertTo(ctx, ci, value, type);
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
                return base.ConvertFrom(ctx, ci, data);
            }
        }
    }
}
