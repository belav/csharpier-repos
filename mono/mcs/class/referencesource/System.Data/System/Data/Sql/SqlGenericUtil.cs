//------------------------------------------------------------------------------
// <copyright file="SqlGenericUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Sql
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;

    internal sealed class SqlGenericUtil
    {
        private SqlGenericUtil()
        { /* prevent utility class from being insantiated*/
        }

        //
        // Sql generic exceptions
        //

        //
        // Sql.Definition
        //

        internal
        //
        // Sql generic exceptions
        //

        //
        // Sql.Definition
        //

        static Exception NullCommandText()
        {
            return ADP.Argument(Res.GetString(Res.Sql_NullCommandText));
        }

        internal static Exception MismatchedMetaDataDirectionArrayLengths()
        {
            return ADP.Argument(Res.GetString(Res.Sql_MismatchedMetaDataDirectionArrayLengths));
        }
    }
} //namespace
