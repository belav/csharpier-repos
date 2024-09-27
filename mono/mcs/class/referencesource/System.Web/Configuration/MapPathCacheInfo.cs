//------------------------------------------------------------------------------
// <copyright file="MapPathCacheInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Configuration
{
    using System.Collections;
    using System.Configuration;
    using System.Globalization;
    using System.Text;
    using System.Web.Caching;
    using System.Web.Hosting;
    using System.Web.Util;
    using Microsoft.Win32;

    internal class MapPathCacheInfo
    {
        internal string MapPathResult;
        internal bool Evaluated;
        internal Exception CachedException;
    }
}
