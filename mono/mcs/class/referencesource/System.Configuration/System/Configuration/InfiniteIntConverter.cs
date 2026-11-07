//------------------------------------------------------------------------------
// <copyright file="InfiniteIntConverter.cs" company="Microsoft">
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
    public sealed class InfiniteIntConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(
            ITypeDescriptorContext ctx,
            CultureInfo ci,
            object value,
            Type type
        )
        {
            ValidateType(value, typeof(int));

            if ((int)value == int.MaxValue)
            {
                return "Infinite";
            }
            else
            {
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            Debug.Assert(data is string, "data is string");

            if ((string)data == "Infinite")
            {
                return int.MaxValue;
            }
            else
            {
                return Convert.ToInt32((string)data, 10);
            }
        }
    }
}
