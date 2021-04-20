﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class FieldsOnlyLoadSqliteTest : FieldsOnlyLoadTestBase<FieldsOnlyLoadSqliteTest.FieldsOnlyLoadSqliteFixture>
    {
        public FieldsOnlyLoadSqliteTest(FieldsOnlyLoadSqliteFixture fixture)
            : base(fixture)
        {
        }

        public class FieldsOnlyLoadSqliteFixture : FieldsOnlyLoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SqliteTestStoreFactory.Instance;
        }
    }
}
