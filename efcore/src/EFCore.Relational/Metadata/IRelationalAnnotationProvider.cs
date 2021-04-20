// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    ///     <para>
    ///         A service typically implemented by database providers that gives access to annotations
    ///         used by relational EF Core components on various elements of the <see cref="IRelationalModel" />.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
    ///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
    ///     </para>
    /// </summary>
    public interface IRelationalAnnotationProvider
    {
        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IRelationalModel" />.
        /// </summary>
        /// <param name="model"> The database model. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IRelationalModel model);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ITable" />.
        /// </summary>
        /// <param name="table"> The table. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ITable table);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IColumn" />.
        /// </summary>
        /// <param name="column"> The column. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IColumn column);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IView" />.
        /// </summary>
        /// <param name="view"> The view. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IView view);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IViewColumn" />.
        /// </summary>
        /// <param name="column"> The column. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IViewColumn column);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ISqlQuery" />.
        /// </summary>
        /// <param name="sqlQuery"> The SQL query. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ISqlQuery sqlQuery);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ISqlQueryColumn" />.
        /// </summary>
        /// <param name="column"> The column. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ISqlQueryColumn column);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IStoreFunction" />.
        /// </summary>
        /// <param name="function"> The function. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IStoreFunction function);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IFunctionColumn" />.
        /// </summary>
        /// <param name="column"> The column. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IFunctionColumn column);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IUniqueConstraint" />.
        /// </summary>
        /// <param name="constraint"> The unique constraint. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IUniqueConstraint constraint);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ITableIndex" />.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ITableIndex index);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="IForeignKeyConstraint" />.
        /// </summary>
        /// <param name="foreignKey"> The foreign key. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(IForeignKeyConstraint foreignKey);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ISequence" />.
        /// </summary>
        /// <param name="sequence"> The sequence. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ISequence sequence);

        /// <summary>
        ///     Gets provider-specific annotations for the given <see cref="ICheckConstraint" />.
        /// </summary>
        /// <param name="checkConstraint"> The check constraint. </param>
        /// <returns> The annotations. </returns>
        IEnumerable<IAnnotation> For(ICheckConstraint checkConstraint);
    }
}
