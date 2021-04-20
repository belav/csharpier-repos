// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Benchmarks.Models.Orders;

namespace Microsoft.EntityFrameworkCore.Benchmarks.Query
{
    public class SimpleQuerySqlServerTests : SimpleQueryTests
    {
        protected override OrdersFixtureBase CreateFixture()
        {
            return new OrdersSqlServerFixture("Perf_Query_Simple");
        }
    }
}
