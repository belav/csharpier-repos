// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ApplicationParts
{
    public class RelatedAssemblyPartTest
    {
        private static readonly string AssemblyDirectory = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar);

        [Fact]
        public void GetRelatedAssemblies_Noops_ForDynamicAssemblies()
        {
            // Arrange
            var name = new AssemblyName($"DynamicAssembly-{Guid.NewGuid()}");
            var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);

            // Act
            var result = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: true);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetRelatedAssemblies_ThrowsIfRelatedAttributeReferencesSelf()
        {
            // Arrange
            var expected = "RelatedAssemblyAttribute specified on MyAssembly cannot be self referential.";
            var assembly = new TestAssembly { AttributeAssembly = "MyAssembly" };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: true));
            Assert.Equal(expected, ex.Message);
        }

        [Fact]
        public void GetRelatedAssemblies_ThrowsIfAssemblyCannotBeFound()
        {
            // Arrange
            var assemblyPath = Path.Combine(AssemblyDirectory, "MyAssembly.dll");
            var assembly = new TestAssembly
            {
                AttributeAssembly = "DoesNotExist"
            };

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: true));
        }

        [Fact]
        public void GetRelatedAssemblies_ReadsAssemblyFromLoadContext_IfItAlreadyExists()
        {
            // Arrange
            var expected = $"Related assembly 'DoesNotExist' specified by assembly 'MyAssembly' could not be found in the directory {AssemblyDirectory}. Related assemblies must be co-located with the specifying assemblies.";
            var assemblyPath = Path.Combine(AssemblyDirectory, "MyAssembly.dll");
            var relatedAssembly = typeof(RelatedAssemblyPartTest).Assembly;
            var assembly = new TestAssembly
            {
                AttributeAssembly = "RelatedAssembly"
            };
            var loadContext = new TestableAssemblyLoadContextWrapper
            {
                Assemblies =
                {
                    ["RelatedAssembly"] = relatedAssembly,
                }
            };

            // Act
            var result = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: true, file => false, loadContext);

            // Assert
            Assert.Equal(new[] { relatedAssembly }, result);
        }

        private class TestAssembly : Assembly
        {
            public override AssemblyName GetName()
            {
                return new AssemblyName("MyAssembly");
            }

            public string AttributeAssembly { get; set; }

            public string LocationSettable { get; set; } = Path.Combine(AssemblyDirectory, "MyAssembly.dll");

            public override string Location => LocationSettable;

            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                var attribute = new RelatedAssemblyAttribute(AttributeAssembly);
                return new[] { attribute };
            }
        }

        private class TestableAssemblyLoadContextWrapper : RelatedAssemblyAttribute.AssemblyLoadContextWrapper
        {
            public TestableAssemblyLoadContextWrapper() : base(AssemblyLoadContext.Default)
            {
            }

            public Dictionary<string, Assembly> Assemblies { get; } = new Dictionary<string, Assembly>();

            public override Assembly LoadFromAssemblyPath(string assemblyPath) => throw new NotSupportedException();

            public override Assembly LoadFromAssemblyName(AssemblyName assemblyName)
            {
                return Assemblies[assemblyName.Name];
            }
        }
    }
}
