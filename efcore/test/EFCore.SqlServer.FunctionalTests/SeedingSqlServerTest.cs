// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class SeedingSqlServerTest : SeedingTestBase
    {
        protected override TestStore TestStore => SqlServerTestStore.Create("SeedingTest");

        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
            => new SeedingSqlServerContext(testId);

        protected class SeedingSqlServerContext : SeedingContext
        {
            public SeedingSqlServerContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(SqlServerTestStore.CreateConnectionString($"Seeds{TestId}"));
        }
    }
}
