//------------------------------------------------------------------------------
// <copyright file="ConfigurationValueFlags.cs" company="Microsoft">
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
    [Flags]
    internal enum ConfigurationValueFlags
    {
        Default = 0,
        Inherited = 1,
        Modified = 2,
        Locked = 4,
        XMLParentInherited = 8,
    }
}
