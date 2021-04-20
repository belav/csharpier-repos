// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Benchmarks.Models.Orders;

#pragma warning disable CA1034 // Nested types should not be visible

namespace Microsoft.EntityFrameworkCore.Benchmarks.ChangeTracker
{
    public class FixupSqliteTests : FixupTests
    {
        public class ChildVariations : ChildVariationsBase
        {
            public override OrdersFixtureBase CreateFixture()
            {
                return new OrdersSqliteFixture("Perf_ChangeTracker_Fixup");
            }
        }

        public class ParentVariations : ParentVariationsBase
        {
            public override OrdersFixtureBase CreateFixture()
            {
                return new OrdersSqliteFixture("Perf_ChangeTracker_Fixup");
            }
        }
    }
}
