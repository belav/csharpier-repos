// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.ValueGeneration
{
    public class ValueGeneratorCacheTest
    {
        [ConditionalFact]
        public void Uses_single_generator_per_property()
        {
            var model = CreateModel();
            var entityType = model.FindEntityType("Led");
            var property1 = entityType.FindProperty("Zeppelin");
            var property2 = entityType.FindProperty("Stairway");
            var cache = InMemoryTestHelpers.Instance.CreateContextServices(model)
                .GetRequiredService<IValueGeneratorCache>();

            var generator1 = cache.GetOrAdd(property1, entityType, (p, et) => new GuidValueGenerator());
            Assert.NotNull(generator1);
            Assert.Same(generator1, cache.GetOrAdd(property1, entityType, (p, et) => new GuidValueGenerator()));

            var generator2 = cache.GetOrAdd(property2, entityType, (p, et) => new GuidValueGenerator());
            Assert.NotNull(generator2);
            Assert.Same(generator2, cache.GetOrAdd(property2, entityType, (p, et) => new GuidValueGenerator()));
            Assert.NotSame(generator1, generator2);
        }

        private static IModel CreateModel(bool generateValues = true)
        {
            var modelBuilder = InMemoryTestHelpers.Instance.CreateConventionBuilder();
            modelBuilder.Entity("Led", eb =>
            {
                eb.Property<int>("Id");
                eb.Property<Guid>("Zeppelin");
                var property = eb.Property<Guid>("Stairway");
                if (generateValues)
                {
                    property.ValueGeneratedOnAdd();
                }
            });

            return modelBuilder.FinalizeModel();
        }
    }
}
