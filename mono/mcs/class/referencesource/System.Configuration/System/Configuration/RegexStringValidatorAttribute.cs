//------------------------------------------------------------------------------
// <copyright file="RegexStringValidatorAttribute.cs" company="Microsoft">
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
    public sealed class RegexStringValidatorAttribute : ConfigurationValidatorAttribute
    {
        private string _regex;

        public RegexStringValidatorAttribute(string regex)
        {
            _regex = regex;
        }

        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new RegexStringValidator(_regex); }
        }
        public string Regex
        {
            get { return _regex; }
        }
    }
}
