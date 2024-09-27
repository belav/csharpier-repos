//------------------------------------------------------------------------------
// <copyright file="SubclassTypeValidatorAttribute.cs" company="Microsoft">
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
    public sealed class SubclassTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type _baseClass;

        public SubclassTypeValidatorAttribute(Type baseClass)
        {
            _baseClass = baseClass;
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new SubclassTypeValidator(_baseClass); }
        }

        public Type BaseClass
        {
            get { return _baseClass; }
        }
    }
}
