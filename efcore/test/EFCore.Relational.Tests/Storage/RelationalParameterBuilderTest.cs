﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public class RelationalParameterBuilderTest
    {
        [ConditionalFact]
        public void Can_add_dynamic_parameter()
        {
            var typeMapper = new TestRelationalTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var parameterBuilder = new RelationalCommandBuilder(
                new RelationalCommandBuilderDependencies(
                    typeMapper));

            parameterBuilder.AddParameter(
                "InvariantName",
                "Name");

            Assert.Equal(1, parameterBuilder.Parameters.Count);

            var parameter = parameterBuilder.Parameters[0] as DynamicRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal("Name", parameter.Name);
        }

        [ConditionalTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_add_type_mapped_parameter_by_type(bool nullable)
        {
            var typeMapper = (IRelationalTypeMappingSource)new TestRelationalTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());
            var typeMapping = typeMapper.FindMapping(nullable ? typeof(int?) : typeof(int));

            var parameterBuilder = new RelationalCommandBuilder(
                new RelationalCommandBuilderDependencies(
                    typeMapper));

            parameterBuilder.AddParameter(
                "InvariantName",
                "Name",
                typeMapping,
                nullable);

            Assert.Equal(1, parameterBuilder.Parameters.Count);

            var parameter = parameterBuilder.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal("Name", parameter.Name);
            Assert.Equal(typeMapping, parameter.RelationalTypeMapping);
            Assert.Equal(nullable, parameter.IsNullable);
        }

        [ConditionalTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_add_type_mapped_parameter_by_property(bool nullable)
        {
            var typeMapper = new TestRelationalTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var model = (IMutableModel)new Model();
            var property = model.AddEntityType("MyType").AddProperty("MyProp", typeof(string));
            property.IsNullable = nullable;

            RelationalTestHelpers.Instance.CreateContextServices().GetRequiredService<IModelRuntimeInitializer>()
                .Initialize(model.FinalizeModel(), designTime: false, validationLogger: null);

            var parameterBuilder = new RelationalCommandBuilder(
                new RelationalCommandBuilderDependencies(typeMapper));

            parameterBuilder.AddParameter(
                "InvariantName",
                "Name",
                property.GetRelationalTypeMapping(),
                property.IsNullable);

            Assert.Equal(1, parameterBuilder.Parameters.Count);

            var parameter = parameterBuilder.Parameters[0] as TypeMappedRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("InvariantName", parameter.InvariantName);
            Assert.Equal("Name", parameter.Name);
            Assert.Equal(property.GetTypeMapping(), parameter.RelationalTypeMapping);
            Assert.Equal(nullable, parameter.IsNullable);
        }

        [ConditionalFact]
        public void Can_add_composite_parameter()
        {
            var typeMapper = new TestRelationalTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var parameterBuilder = new RelationalCommandBuilder(
                new RelationalCommandBuilderDependencies(
                    typeMapper));

            parameterBuilder.AddCompositeParameter(
                "CompositeInvariant",
                new List<IRelationalParameter>
                {
                    new TypeMappedRelationalParameter(
                        "FirstInvariant",
                        "FirstName",
                        new IntTypeMapping("int", DbType.Int32),
                        nullable: false),
                    new TypeMappedRelationalParameter(
                        "SecondInvariant",
                        "SecondName",
                        new StringTypeMapping("nvarchar(max)"),
                        nullable: true)
                });

            Assert.Equal(1, parameterBuilder.Parameters.Count);

            var parameter = parameterBuilder.Parameters[0] as CompositeRelationalParameter;

            Assert.NotNull(parameter);
            Assert.Equal("CompositeInvariant", parameter.InvariantName);
            Assert.Equal(2, parameter.RelationalParameters.Count);
        }

        [ConditionalFact]
        public void Does_not_add_empty_composite_parameter()
        {
            var typeMapper = new TestRelationalTypeMappingSource(
                TestServiceFactory.Instance.Create<TypeMappingSourceDependencies>(),
                TestServiceFactory.Instance.Create<RelationalTypeMappingSourceDependencies>());

            var parameterBuilder = new RelationalCommandBuilder(
                new RelationalCommandBuilderDependencies(
                    typeMapper));

            parameterBuilder.AddCompositeParameter(
                "CompositeInvariant",
                new List<IRelationalParameter>());

            Assert.Equal(0, parameterBuilder.Parameters.Count);
        }

        public static RelationalTypeMapping GetMapping(
            IRelationalTypeMappingSource typeMappingSource,
            IProperty property)
            => typeMappingSource.FindMapping(property);
    }
}
