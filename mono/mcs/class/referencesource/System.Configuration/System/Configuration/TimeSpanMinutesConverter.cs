//------------------------------------------------------------------------------
// <copyright file="TimeSpanMinutesConverter.cs" company="Microsoft">
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
    public class TimeSpanMinutesConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(
            ITypeDescriptorContext ctx,
            CultureInfo ci,
            object value,
            Type type
        )
        {
            ValidateType(value, typeof(TimeSpan));

            long data = (long)(((TimeSpan)value).TotalMinutes);

            return data.ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            Debug.Assert(data is string, "data is string");

            long min = long.Parse((string)data, CultureInfo.InvariantCulture);

            return TimeSpan.FromMinutes((double)min);
        }
    }
}
