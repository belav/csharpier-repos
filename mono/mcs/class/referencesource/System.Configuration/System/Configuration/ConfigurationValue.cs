//------------------------------------------------------------------------------
// <copyright file="ConfigurationValue.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Internal;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace System.Configuration
{
    internal class ConfigurationValue
    {
        internal ConfigurationValueFlags ValueFlags;
        internal object Value;
        internal PropertySourceInfo SourceInfo;

        internal ConfigurationValue(
            object value,
            ConfigurationValueFlags valueFlags,
            PropertySourceInfo sourceInfo
        )
        {
            Value = value;
            ValueFlags = valueFlags;
            SourceInfo = sourceInfo;
        }
    }
}
