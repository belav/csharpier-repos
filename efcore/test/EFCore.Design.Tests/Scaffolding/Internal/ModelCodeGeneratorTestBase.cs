﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public abstract class ModelCodeGeneratorTestBase
    {
        protected void Test(
            Action<ModelBuilder> buildModel,
            ModelCodeGenerationOptions options,
            Action<ScaffoldedModel> assertScaffold,
            Action<IModel> assertModel)
        {
            var modelBuilder = SqlServerTestHelpers.Instance.CreateConventionBuilder();
            modelBuilder.Model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);
            buildModel(modelBuilder);

            var model = SqlServerTestHelpers.Instance.Finalize(modelBuilder, designTime: true, skipValidation: true);

            var generator = CreateServices()
                .BuildServiceProvider()
                .GetRequiredService<IModelCodeGenerator>();

            options.ModelNamespace ??= "TestNamespace";
            options.ContextName = "TestDbContext";
            options.ConnectionString = "Initial Catalog=TestDatabase";

            var scaffoldedModel = generator.GenerateModel(
                model,
                options);
            assertScaffold(scaffoldedModel);

            var build = new BuildSource
            {
                References =
                {
                    BuildReference.ByName("Microsoft.EntityFrameworkCore.Abstractions"),
                    BuildReference.ByName("Microsoft.EntityFrameworkCore"),
                    BuildReference.ByName("Microsoft.EntityFrameworkCore.Relational"),
                    BuildReference.ByName("Microsoft.EntityFrameworkCore.SqlServer")
                },
                Sources = new List<string>(
                    new[] { scaffoldedModel.ContextFile.Code }.Concat(
                        scaffoldedModel.AdditionalFiles.Select(f => f.Code)))
            };

            var assembly = build.BuildInMemory();
            var contextNamespace = options.ContextNamespace ?? options.ModelNamespace;
            var context = (DbContext)assembly.CreateInstance(
                !string.IsNullOrEmpty(contextNamespace)
                    ? contextNamespace + "." + options.ContextName
                    : options.ContextName);

            if (assertModel != null)
            {
                var compiledModel = context.Model;
                assertModel(compiledModel);
            }
        }

        protected static IServiceCollection CreateServices()
        {
            var testAssembly = typeof(ModelCodeGeneratorTestBase).Assembly;
            var reporter = new TestOperationReporter();
            var services = new DesignTimeServicesBuilder(testAssembly, testAssembly, reporter, new string[0])
                .CreateServiceCollection("Microsoft.EntityFrameworkCore.SqlServer");
            return services;
        }

        protected static void AssertFileContents(
            string expectedCode,
            ScaffoldedFile file)
            => Assert.Equal(expectedCode, file.Code, ignoreLineEndingDifferences: true);
    }
}
