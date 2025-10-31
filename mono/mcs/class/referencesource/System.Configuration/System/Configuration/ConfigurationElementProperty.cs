//------------------------------------------------------------------------------
// <copyright file="ConfigurationElementProperty.cs" company="Microsoft">
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
    //  Although in Whidbey this class contain just one property, but we still do this this way,
    // instead of exposing a Validator property in ConfigurationElement, because if we need
    // another property in the future we'll expand this ElementProperty class rather than adding a
    // new overridable on ConfigurationElement
    public sealed class ConfigurationElementProperty
    {
        private ConfigurationValidatorBase _validator;

        public ConfigurationElementProperty(ConfigurationValidatorBase validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }

            _validator = validator;
        }

        public ConfigurationValidatorBase Validator
        {
            get { return _validator; }
        }
    }
}
