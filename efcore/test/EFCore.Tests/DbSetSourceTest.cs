// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class DbSetSourceTest
    {
        [ConditionalFact]
        public void Can_create_new_generic_DbSet()
        {
            var context = InMemoryTestHelpers.Instance.CreateContext();

            var factorySource = new DbSetSource();

            var set = factorySource.Create(context, typeof(Random));

            Assert.IsType<InternalDbSet<Random>>(set);
        }

        [ConditionalFact]
        public void Always_creates_a_new_DbSet_instance()
        {
            var context = InMemoryTestHelpers.Instance.CreateContext();

            var factorySource = new DbSetSource();

            Assert.NotSame(factorySource.Create(context, typeof(Random)), factorySource.Create(context, typeof(Random)));
        }

        [ConditionalFact]
        public void Can_create_new_generic_DbSet_for_shared_type()
        {
            var context = InMemoryTestHelpers.Instance.CreateContext();

            var factorySource = new DbSetSource();

            var set = factorySource.Create(context, nameof(Random), typeof(Random));

            Assert.IsType<InternalDbSet<Random>>(set);
        }

        [ConditionalFact]
        public void Always_creates_a_new_DbSet_instance_for_shared_type()
        {
            var context = InMemoryTestHelpers.Instance.CreateContext();

            var factorySource = new DbSetSource();

            Assert.NotSame(
                factorySource.Create(context, nameof(Random), typeof(Random)),
                factorySource.Create(context, nameof(Random), typeof(Random)));
        }
    }
}
