// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class InheritanceRelationshipsQuerySqliteTest :
        InheritanceRelationshipsQueryRelationalTestBase<InheritanceRelationshipsQuerySqliteTest.InheritanceRelationshipsQuerySqliteFixture>
    {
        public InheritanceRelationshipsQuerySqliteTest(InheritanceRelationshipsQuerySqliteFixture fixture)
            : base(fixture)
        {
        }

        public class InheritanceRelationshipsQuerySqliteFixture : InheritanceRelationshipsQueryRelationalFixture
        {
            protected override ITestStoreFactory TestStoreFactory
                => SqliteTestStoreFactory.Instance;
        }
    }
}
