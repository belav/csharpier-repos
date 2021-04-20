// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
    /// <summary>
    ///     A simple model for a database used when reverse engineering an existing database.
    /// </summary>
    public class DatabaseModel : Annotatable
    {
        /// <summary>
        ///     The database name, or <see langword="null" /> if none is set.
        /// </summary>
        public virtual string? DatabaseName { get; set; }

        /// <summary>
        ///     The database schema, or <see langword="null" /> to use the default schema.
        /// </summary>
        public virtual string? DefaultSchema { get; set; }

        /// <summary>
        ///     The database collation, or <see langword="null" /> if none is set.
        /// </summary>
        public virtual string? Collation { get; set; }

        /// <summary>
        ///     The list of tables in the database.
        /// </summary>
        public virtual IList<DatabaseTable> Tables { get; } = new List<DatabaseTable>();

        /// <summary>
        ///     The list of sequences in the database.
        /// </summary>
        public virtual IList<DatabaseSequence> Sequences { get; } = new List<DatabaseSequence>();
    }
}
