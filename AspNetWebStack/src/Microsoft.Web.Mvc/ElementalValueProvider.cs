// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.Web.Mvc
{
    // Represents a value provider that contains a single value.
    internal sealed class ElementalValueProvider : IValueProvider
    {
        public ElementalValueProvider(string name, object rawValue, CultureInfo culture)
        {
            Name = name;
            RawValue = rawValue;
            Culture = culture;
        }

        public CultureInfo Culture { get; private set; }

        public string Name { get; private set; }

        public object RawValue { get; private set; }

        public bool ContainsPrefix(string prefix)
        {
            return ValueProviderUtil.IsPrefixMatch(prefix, Name);
        }

        public ValueProviderResult GetValue(string key)
        {
            return (String.Equals(key, Name, StringComparison.OrdinalIgnoreCase))
                       ? new ValueProviderResult(RawValue, Convert.ToString(RawValue, Culture), Culture)
                       : null;
        }
    }
}
