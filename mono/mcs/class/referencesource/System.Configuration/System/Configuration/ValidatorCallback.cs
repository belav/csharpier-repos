//------------------------------------------------------------------------------
// <copyright file="ValidatorCallback.cs" company="Microsoft">
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
    // Call back validator. Uses a validation callback to avoid creation of new types
    public delegate void ValidatorCallback(object value);
}
