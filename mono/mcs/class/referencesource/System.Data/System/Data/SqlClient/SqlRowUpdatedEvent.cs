//------------------------------------------------------------------------------
// <copyright file="SqlRowUpdatedEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    public sealed class SqlRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public SqlRowUpdatedEventArgs(
            DataRow row,
            IDbCommand command,
            StatementType statementType,
            DataTableMapping tableMapping
        )
            : base(row, command, statementType, tableMapping) { }

        public new SqlCommand Command
        {
            get { return (SqlCommand)base.Command; }
        }
    }
}
