// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SqlQueryColumnMapping : ColumnMappingBase, ISqlQueryColumnMapping
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public SqlQueryColumnMapping(
            IProperty property,
            SqlQueryColumn column,
            SqlQueryMapping sqlQueryMapping)
            : base(property, column, sqlQueryMapping)
        {
        }

        /// <inheritdoc />
        public virtual ISqlQueryMapping SqlQueryMapping
            => (ISqlQueryMapping)TableMapping;

        /// <inheritdoc />
        public override RelationalTypeMapping TypeMapping => Property.FindRelationalTypeMapping(
            StoreObjectIdentifier.SqlQuery(SqlQueryMapping.SqlQuery.Name))!;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override string ToString()
            => ((ISqlQueryColumnMapping)this).ToDebugString(MetadataDebugStringOptions.SingleLineDefault);

        /// <inheritdoc />
        ISqlQueryColumn ISqlQueryColumnMapping.Column
        {
            [DebuggerStepThrough]
            get => (ISqlQueryColumn)Column;
        }
    }
}
