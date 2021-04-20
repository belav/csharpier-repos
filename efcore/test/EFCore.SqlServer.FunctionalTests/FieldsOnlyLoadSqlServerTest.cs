﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore
{
    public class FieldsOnlyLoadSqlServerTest : FieldsOnlyLoadTestBase<FieldsOnlyLoadSqlServerTest.FieldsOnlyLoadSqlServerFixture>
    {
        public FieldsOnlyLoadSqlServerTest(FieldsOnlyLoadSqlServerFixture fixture)
            : base(fixture)
        {
        }

        public class FieldsOnlyLoadSqlServerFixture : FieldsOnlyLoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => SqlServerTestStoreFactory.Instance;
        }
    }
}
