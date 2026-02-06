//------------------------------------------------------------------------------
// <copyright file="TimeSpanValidatorAttribute.cs" company="Microsoft">
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
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositiveTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
    {
        public PositiveTimeSpanValidatorAttribute() { }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new PositiveTimeSpanValidator(); }
        }
    }
}
