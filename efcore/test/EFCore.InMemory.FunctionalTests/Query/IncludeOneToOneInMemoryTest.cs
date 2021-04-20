// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class IncludeOneToOneInMemoryTest : IncludeOneToOneTestBase<IncludeOneToOneInMemoryTest.OneToOneQueryInMemoryFixture>
    {
        public IncludeOneToOneInMemoryTest(OneToOneQueryInMemoryFixture fixture)
            : base(fixture)
        {
        }

        public class OneToOneQueryInMemoryFixture : OneToOneQueryFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => InMemoryTestStoreFactory.Instance;
        }
    }
}
