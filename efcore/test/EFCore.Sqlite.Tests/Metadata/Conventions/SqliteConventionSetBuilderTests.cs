// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class SqliteConventionSetBuilderTests : ConventionSetBuilderTests
    {
        public override IReadOnlyModel Can_build_a_model_with_default_conventions_without_DI()
        {
            var model = base.Can_build_a_model_with_default_conventions_without_DI();

            Assert.Equal("ProductTable", model.GetEntityTypes().Single().GetTableName());

            return model;
        }

        protected override ConventionSet GetConventionSet()
            => SqliteConventionSetBuilder.Build();

        protected override ModelBuilder GetModelBuilder()
            => SqliteConventionSetBuilder.CreateModelBuilder();
    }
}
