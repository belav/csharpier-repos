// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc.Filters;

namespace BasicWebSite;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class VndErrorAttribute : Attribute, IFilterMetadata { }
