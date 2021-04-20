// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Entity type extension methods for SQL Server-specific metadata.
    /// </summary>
    public static class SqlServerEntityTypeExtensions
    {
        /// <summary>
        ///     Returns a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> <see langword="true" /> if the entity type is mapped to a memory-optimized table. </returns>
        public static bool IsMemoryOptimized(this IReadOnlyEntityType entityType)
            => entityType[SqlServerAnnotationNames.MemoryOptimized] as bool? ?? false;

        /// <summary>
        ///     Sets a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="memoryOptimized"> The value to set. </param>
        public static void SetIsMemoryOptimized(this IMutableEntityType entityType, bool memoryOptimized)
            => entityType.SetOrRemoveAnnotation(SqlServerAnnotationNames.MemoryOptimized, memoryOptimized);

        /// <summary>
        ///     Sets a value indicating whether the entity type is mapped to a memory-optimized table.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="memoryOptimized"> The value to set. </param>
        /// <param name="fromDataAnnotation"> Indicates whether the configuration was specified using a data annotation. </param>
        /// <returns> The configured value. </returns>
        public static bool? SetIsMemoryOptimized(
            this IConventionEntityType entityType,
            bool? memoryOptimized,
            bool fromDataAnnotation = false)
        {
            entityType.SetOrRemoveAnnotation(SqlServerAnnotationNames.MemoryOptimized, memoryOptimized, fromDataAnnotation);

            return memoryOptimized;
        }

        /// <summary>
        ///     Gets the configuration source for the memory-optimized setting.
        /// </summary>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The configuration source for the memory-optimized setting. </returns>
        public static ConfigurationSource? GetIsMemoryOptimizedConfigurationSource(this IConventionEntityType entityType)
            => entityType.FindAnnotation(SqlServerAnnotationNames.MemoryOptimized)?.GetConfigurationSource();
    }
}
