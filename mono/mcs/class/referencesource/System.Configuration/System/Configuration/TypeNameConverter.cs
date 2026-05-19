//------------------------------------------------------------------------------
// <copyright file="TypeNameConverter.cs" company="Microsoft">
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
    public sealed class TypeNameConverter : ConfigurationConverterBase
    {
        public override object ConvertTo(
            ITypeDescriptorContext ctx,
            CultureInfo ci,
            object value,
            Type type
        )
        {
            // Make the check here since for some reason value.GetType is not System.Type but RuntimeType
            if (!(value is Type))
            {
                ValidateType(value, typeof(Type));
            }

            string result = null;

            if (value != null)
            {
                result = ((Type)value).AssemblyQualifiedName;
            }

            return result;
        }

        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            Type result = TypeUtil.GetTypeWithReflectionPermission((string)data, false);

            if (result == null)
            {
                throw new ArgumentException(SR.GetString(SR.Type_cannot_be_resolved, (string)data));
            }

            return result;
        }
    }
}
