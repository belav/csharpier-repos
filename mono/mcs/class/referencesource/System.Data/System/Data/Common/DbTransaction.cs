//------------------------------------------------------------------------------
// <copyright file="DbTransaction.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.Data;

    public abstract class DbTransaction : MarshalByRefObject, IDbTransaction
    { // V1.2.3300
        protected DbTransaction()
            : base() { }

        public DbConnection Connection
        {
            get { return DbConnection; }
        }

        IDbConnection IDbTransaction.Connection
        {
            get { return DbConnection; }
        }

        protected abstract DbConnection DbConnection { get; }

        public abstract IsolationLevel IsolationLevel { get; }

        public abstract void Commit();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) { }

        public abstract void Rollback();
    }
}
