// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Migrations.Operations
{
    /// <summary>
    ///     A <see cref="MigrationOperation" /> for renaming an existing sequence.
    /// </summary>
    [DebuggerDisplay("ALTER SEQUENCE {Name} RENAME TO {NewName}")]
    public class RenameSequenceOperation : MigrationOperation
    {
        /// <summary>
        ///     The old name of the sequence.
        /// </summary>
        public virtual string Name { get; set; } = null!;

        /// <summary>
        ///     The schema that contains the sequence, or <see langword="null" /> if the default schema should be used.
        /// </summary>
        public virtual string? Schema { get; set; }

        /// <summary>
        ///     The new sequence name or <see langword="null" /> if only the schema has changed.
        /// </summary>
        public virtual string? NewName { get; set; }

        /// <summary>
        ///     The new schema name or <see langword="null" /> if only the name has changed.
        /// </summary>
        public virtual string? NewSchema { get; set; }
    }
}
