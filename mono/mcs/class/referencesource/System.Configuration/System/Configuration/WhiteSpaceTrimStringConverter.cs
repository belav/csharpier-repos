//------------------------------------------------------------------------------
// <copyright file="WhiteSpaceTrimStringConverter.cs" company="Microsoft">
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
    public sealed class WhiteSpaceTrimStringConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(
            ITypeDescriptorContext ctx,
            CultureInfo ci,
            object value,
            Type type
        )
        {
            ValidateType(value, typeof(string));

            if (value == null)
            {
                return String.Empty;
            }

            return ((string)value).Trim();
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            Debug.Assert(data is string, "data is string");
            return ((string)data).Trim();
        }
    }
}
