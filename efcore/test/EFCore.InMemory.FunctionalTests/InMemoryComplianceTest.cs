﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore
{
    public class InMemoryComplianceTest : ComplianceTestBase
    {
        protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
        {
            // No in-memory tests
            typeof(FunkyDataQueryTestBase<>),
            typeof(StoreGeneratedTestBase<>),
            typeof(ConferencePlannerTestBase<>),
            typeof(ManyToManyQueryTestBase<>),
        };

        protected override Assembly TargetAssembly { get; } = typeof(InMemoryComplianceTest).Assembly;
    }
}
