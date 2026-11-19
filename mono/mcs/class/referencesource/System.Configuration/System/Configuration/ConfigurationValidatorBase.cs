//------------------------------------------------------------------------------
// <copyright file="ConfigurationValidatorBase.cs" company="Microsoft">
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
using System.Text.RegularExpressions;
using System.Xml;

namespace System.Configuration
{
    public abstract class ConfigurationValidatorBase
    {
        public virtual bool CanValidate(Type type)
        {
            return false;
        }

        public abstract void Validate(object value);
    }
}
