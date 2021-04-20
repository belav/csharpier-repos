﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public abstract class ComplexNavigationsSharedTypeQueryTestBase<TFixture> : ComplexNavigationsQueryTestBase<TFixture>
        where TFixture : ComplexNavigationsSharedTypeQueryFixtureBase, new()
    {
        protected ComplexNavigationsSharedTypeQueryTestBase(TFixture fixture)
            : base(fixture)
        {
        }

        // Self-ref not supported
        public override Task Join_navigation_self_ref(bool async)
        {
            return Task.CompletedTask;
        }

        public override Task Multiple_complex_includes_self_ref(bool async)
        {
            return Task.CompletedTask;
        }

        public override Task Join_condition_optimizations_applied_correctly_when_anonymous_type_with_multiple_properties(bool async)
        {
            return Task.CompletedTask;
        }

        public override Task Join_condition_optimizations_applied_correctly_when_anonymous_type_with_single_property(bool async)
        {
            return Task.CompletedTask;
        }

        [ConditionalTheory(Skip = "issue #13560")]
        public override Task Multiple_SelectMany_with_nested_navigations_and_explicit_DefaultIfEmpty_joined_together(bool async)
        {
            return base.Multiple_SelectMany_with_nested_navigations_and_explicit_DefaultIfEmpty_joined_together(async);
        }

        [ConditionalTheory(Skip = "issue #13560")]
        public override Task SelectMany_with_nested_navigations_explicit_DefaultIfEmpty_and_additional_joins_outside_of_SelectMany(
            bool async)
        {
            return base.SelectMany_with_nested_navigations_explicit_DefaultIfEmpty_and_additional_joins_outside_of_SelectMany(async);
        }

        [ConditionalTheory(Skip = "issue #13560")]
        public override Task SelectMany_with_nested_navigations_explicit_DefaultIfEmpty_and_additional_joins_outside_of_SelectMany2(
            bool async)
        {
            return base.SelectMany_with_nested_navigations_explicit_DefaultIfEmpty_and_additional_joins_outside_of_SelectMany2(async);
        }

        [ConditionalTheory(Skip = "issue #13560")]
        public override Task SelectMany_with_nested_navigations_and_additional_joins_outside_of_SelectMany(bool async)
        {
            return base.SelectMany_with_nested_navigations_and_additional_joins_outside_of_SelectMany(async);
        }

        [ConditionalTheory(Skip = "issue #13560")]
        public override Task
            Complex_SelectMany_with_nested_navigations_and_explicit_DefaultIfEmpty_with_other_query_operators_composed_on_top(bool async)
        {
            return base.Complex_SelectMany_with_nested_navigations_and_explicit_DefaultIfEmpty_with_other_query_operators_composed_on_top(
                async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include8(bool async)
        {
            return base.Include8(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include9(bool async)
        {
            return base.Include9(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_complex(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_complex(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_complex_repeated(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_complex_repeated(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_complex_repeated_checked(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_complex_repeated_checked(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_member(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_member(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_methodcall(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_methodcall(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_collection_with_multiple_orderbys_property(bool async)
        {
            return base.Include_collection_with_multiple_orderbys_property(async);
        }

        [ConditionalTheory(Skip = "Issue#17803")]
        public override Task Member_pushdown_with_multiple_collections(bool async)
        {
            return base.Member_pushdown_with_multiple_collections(async);
        }

        // Cannot create DbSet for Level2
        public override void Join_with_navigations_in_the_result_selector2()
        {
        }

        public override void Member_pushdown_chain_3_levels_deep()
        {
        }

        public override void Member_pushdown_chain_3_levels_deep_entity()
        {
        }

        public override void Member_pushdown_with_collection_navigation_in_the_middle()
        {
        }

        public override Task Union_over_entities_with_different_nullability(bool async)
            => Task.CompletedTask;

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Include_inside_subquery(bool async)
        {
            return base.Include_inside_subquery(async);
        }

        [ConditionalTheory(Skip = "Issue#16752")]
        public override Task Filtered_include_outer_parameter_used_inside_filter(bool async)
        {
            return base.Filtered_include_outer_parameter_used_inside_filter(async);
        }

        [ConditionalTheory(Skip = "Issue#17803")]
        public override Task Multiple_collection_FirstOrDefault_followed_by_member_access_in_projection(bool async)
        {
            return base.Multiple_collection_FirstOrDefault_followed_by_member_access_in_projection(async);
        }

        public override Task Project_shadow_properties(bool async)
            => Task.CompletedTask;

        public override Task SelectMany_DefaultIfEmpty_multiple_times_with_joins_projecting_a_collection(bool async)
        {
            // Navigations used are not mapped in shared type.
            return Task.CompletedTask;
        }
    }
}
