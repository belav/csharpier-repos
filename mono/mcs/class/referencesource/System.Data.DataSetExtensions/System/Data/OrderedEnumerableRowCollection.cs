//------------------------------------------------------------------------------
// <copyright file="OrderedEnumerableRowCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// This class provides a wrapper for DataTables representing an ordered sequence.
    /// </summary>
    public sealed class OrderedEnumerableRowCollection<TRow> : EnumerableRowCollection<TRow>
    {
        /// <summary>
        /// Copy Constructor that sets enumerableRows to the one given in the input
        /// </summary>
        internal OrderedEnumerableRowCollection(
            EnumerableRowCollection<TRow> enumerableTable,
            IEnumerable<TRow> enumerableRows
        )
            : base(enumerableTable, enumerableRows, null) { }
    }
}
