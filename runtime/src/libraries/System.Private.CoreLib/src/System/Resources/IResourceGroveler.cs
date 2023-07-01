using System.Collections.Generic;
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*============================================================
**
**
**
**
**
** Purpose: Interface for resource grovelers
**
**
===========================================================*/

using System.Globalization;

namespace System.Resources
{
    internal interface IResourceGroveler
    {
        ResourceSet? GrovelForResourceSet(
            CultureInfo culture,
            Dictionary<string, ResourceSet> localResourceSets,
            bool tryParents,
            bool createIfNotExists
        );
    }
}
