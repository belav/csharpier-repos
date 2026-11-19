//------------------------------------------------------------------------------
// <copyright file="RegexStringValidator.cs" company="Microsoft">
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
    public class RegexStringValidator : ConfigurationValidatorBase
    {
        private string _expression;
        private Regex _regex;

        public RegexStringValidator(string regex)
        {
            if (string.IsNullOrEmpty(regex))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("regex");
            }

            _expression = regex;
            _regex = new Regex(regex, RegexOptions.Compiled);
        }

        public override bool CanValidate(Type type)
        {
            return (type == typeof(string));
        }

        public override void Validate(object value)
        {
            ValidatorUtils.HelperParamValidation(value, typeof(string));

            if (value == null)
            {
                return;
            }

            Match match = _regex.Match((string)value);

            if (!match.Success)
            {
                throw new ArgumentException(SR.GetString(SR.Regex_validator_error, _expression));
            }
        }
    }
}
