// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class NorthwindAsNoTrackingQuerySqlServerTest : NorthwindAsNoTrackingQueryTestBase<
        NorthwindQuerySqlServerFixture<NoopModelCustomizer>>
    {
        public NorthwindAsNoTrackingQuerySqlServerTest(
            NorthwindQuerySqlServerFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
