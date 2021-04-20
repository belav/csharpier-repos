// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    /// <summary>
    ///     <para>
    ///         An implementation of <see cref="IModelSource" /> that produces a model based on
    ///         the <see cref="DbSet{TEntity}" /> properties exposed on the context. The model is cached to avoid
    ///         recreating it every time it is requested.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public class ModelSource : IModelSource
    {
        private readonly object _syncObject = new();

        /// <summary>
        ///     Creates a new <see cref="ModelSource" /> instance.
        /// </summary>
        /// <param name="dependencies"> The dependencies to use. </param>
        public ModelSource(ModelSourceDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="ModelSource" />
        /// </summary>
        protected virtual ModelSourceDependencies Dependencies { get; }

        /// <summary>
        ///     Returns the model from the cache, or creates a model if it is not present in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <returns> The model to be used. </returns>
        [Obsolete("Use the overload with IModelCreationDependencies")]
        public virtual IModel GetModel(
            DbContext context,
            IConventionSetBuilder conventionSetBuilder)
        {
            var cache = Dependencies.MemoryCache;
            var cacheKey = Dependencies.ModelCacheKeyFactory.Create(context);
            if (!cache.TryGetValue(cacheKey, out IModel model))
            {
                // Make sure OnModelCreating really only gets called once, since it may not be thread safe.
                lock (_syncObject)
                {
                    if (!cache.TryGetValue(cacheKey, out model))
                    {
                        model = CreateModel(context, conventionSetBuilder);
                        model = cache.Set(cacheKey, model, new MemoryCacheEntryOptions { Size = 100, Priority = CacheItemPriority.High });
                    }
                }
            }

            return model;
        }

        /// <summary>
        ///     Returns the model from the cache, or creates a model if it is not present in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <param name="modelDependencies"> The dependencies object for the model. </param>
        /// <returns> The model to be used. </returns>
        [Obsolete("Use the overload with IModelCreationDependencies")]
        public virtual IModel GetModel(
            DbContext context,
            IConventionSetBuilder conventionSetBuilder,
            ModelDependencies modelDependencies)
        {
            var cache = Dependencies.MemoryCache;
            var cacheKey = Dependencies.ModelCacheKeyFactory.Create(context);
            if (!cache.TryGetValue(cacheKey, out IModel model))
            {
                // Make sure OnModelCreating really only gets called once, since it may not be thread safe.
                lock (_syncObject)
                {
                    if (!cache.TryGetValue(cacheKey, out model))
                    {
                        model = CreateModel(context, conventionSetBuilder, modelDependencies);
                        model = cache.Set(cacheKey, model, new MemoryCacheEntryOptions { Size = 100, Priority = CacheItemPriority.High });
                    }
                }
            }

            return model;
        }

        /// <summary>
        ///     Gets the model to be used.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="modelCreationDependencies"> The dependencies object used during the creation of the model. </param>
        /// <param name="designTime"> Whether the model should contain design-time configuration.</param>
        /// <returns> The model to be used. </returns>
        public virtual IModel GetModel(
            DbContext context,
            ModelCreationDependencies modelCreationDependencies,
            bool designTime)
        {
            var cache = Dependencies.MemoryCache;
            var cacheKey = Dependencies.ModelCacheKeyFactory.Create(context, designTime);
            if (!cache.TryGetValue(cacheKey, out IModel model))
            {
                // Make sure OnModelCreating really only gets called once, since it may not be thread safe.
                lock (_syncObject)
                {
                    if (!cache.TryGetValue(cacheKey, out model))
                    {
                        model = CreateModel(context, modelCreationDependencies.ConventionSetBuilder, modelCreationDependencies.ModelDependencies);

                        model = modelCreationDependencies.ModelRuntimeInitializer.Initialize(
                            model, designTime, modelCreationDependencies.ValidationLogger);

                        model = cache.Set(cacheKey, model, new MemoryCacheEntryOptions { Size = 100, Priority = CacheItemPriority.High });
                    }
                }
            }

            return model;
        }

        /// <summary>
        ///     Creates the model. This method is called when the model was not found in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <returns> The model to be used. </returns>
        [Obsolete("Use the overload with IModelCreationDependencies")]
        protected virtual IModel CreateModel(
            DbContext context,
            IConventionSetBuilder conventionSetBuilder)
        {
            Check.NotNull(context, nameof(context));

            var modelBuilder = new ModelBuilder(conventionSetBuilder.CreateConventionSet());

            Dependencies.ModelCustomizer.Customize(modelBuilder, context);

            return modelBuilder.FinalizeModel();
        }

        /// <summary>
        ///     Creates the model. This method is called when the model was not found in the cache.
        /// </summary>
        /// <param name="context"> The context the model is being produced for. </param>
        /// <param name="conventionSetBuilder"> The convention set to use when creating the model. </param>
        /// <param name="modelDependencies"> The dependencies object for the model. </param>
        /// <returns> The model to be used. </returns>
        protected virtual IModel CreateModel(
            DbContext context,
            IConventionSetBuilder conventionSetBuilder,
            ModelDependencies modelDependencies)
        {
            Check.DebugAssert(context != null, "context == null");

            var modelBuilder = new ModelBuilder(conventionSetBuilder.CreateConventionSet(), modelDependencies);

            Dependencies.ModelCustomizer.Customize(modelBuilder, context);

            return modelBuilder.FinalizeModel();
        }
    }
}
