//------------------------------------------------------------------------------
// <copyright file="CallbackValidator.cs" company="Microsoft">
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
    public sealed class CallbackValidator : ConfigurationValidatorBase
    {
        Type _type;
        ValidatorCallback _callback;

        public CallbackValidator(Type type, ValidatorCallback callback)
            : this(callback)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            _type = type;
        }

        // Do not check for null type here to handle the callback attribute case
        internal CallbackValidator(ValidatorCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            _type = null;
            _callback = callback;
        }

        public override bool CanValidate(Type type)
        {
            return (type == _type || _type == null);
        }

        public override void Validate(object value)
        {
            _callback(value);
        }
    }
}
