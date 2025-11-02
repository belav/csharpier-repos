//------------------------------------------------------------------------------
// <copyright file="DbConnection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DbConnection : Component, IDbConnection
    { // V1.2.3300
        private StateChangeEventHandler _stateChangeEventHandler;

        protected DbConnection()
            : base() { }

        [
            DefaultValue(""),
#pragma warning disable 618 // ignore obsolete warning about RecommendedAsConfigurable to use SettingsBindableAttribute
            RecommendedAsConfigurable(true),
#pragma warning restore 618
            SettingsBindableAttribute(true),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
        ]
        public abstract string ConnectionString { get; set; }

        [ResCategoryAttribute(Res.DataCategory_Data)]
        public virtual int ConnectionTimeout
        {
            get { return ADP.DefaultConnectionTimeout; }
        }

        [ResCategoryAttribute(Res.DataCategory_Data)]
        public abstract string Database { get; }

        [ResCategoryAttribute(Res.DataCategory_Data)]
        public abstract string DataSource {
            // NOTE: if you plan on allowing the data source to be changed, you
            //       should implement a ChangeDataSource method, in keeping with
            //       the ChangeDatabase method paradigm.
            get; }

        /// <summary>
        /// The associated provider factory for derived class.
        /// </summary>
        virtual protected DbProviderFactory DbProviderFactory
        {
            get { return null; }
        }

        internal DbProviderFactory ProviderFactory
        {
            get { return DbProviderFactory; }
        }

        [Browsable(false)]
        public abstract string ServerVersion { get; }

        [Browsable(false), ResDescriptionAttribute(Res.DbConnection_State)]
        public abstract ConnectionState State { get; }

        [
            ResCategoryAttribute(Res.DataCategory_StateChange),
            ResDescriptionAttribute(Res.DbConnection_StateChange),
        ]
        public virtual event StateChangeEventHandler StateChange
        {
            add { _stateChangeEventHandler += value; }
            remove { _stateChangeEventHandler -= value; }
        }

        protected abstract DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

        public DbTransaction BeginTransaction()
        {
            return BeginDbTransaction(IsolationLevel.Unspecified);
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginDbTransaction(isolationLevel);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return BeginDbTransaction(IsolationLevel.Unspecified);
        }

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginDbTransaction(isolationLevel);
        }

        public abstract void Close();

        public abstract void ChangeDatabase(string databaseName);

        public DbCommand CreateCommand()
        {
            return CreateDbCommand();
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            return CreateDbCommand();
        }

        protected abstract DbCommand CreateDbCommand();

        public virtual void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            // NOTE: This is virtual because not all providers may choose to support
            //       distributed transactions.
            throw ADP.NotSupported();
        }

        // these need to be here so that GetSchema is visible when programming to a dbConnection object.
        // they are overridden by the real implementations in DbConnectionBase
        virtual public DataTable GetSchema()
        {
            throw ADP.NotSupported();
        }

        public virtual DataTable GetSchema(string collectionName)
        {
            throw ADP.NotSupported();
        }

        public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            throw ADP.NotSupported();
        }

        internal bool _supressStateChangeForReconnection = false; // Do not use for anything else ! Value will be overwritten by CR process

        protected virtual void OnStateChange(StateChangeEventArgs stateChange)
        {
            if (_supressStateChangeForReconnection)
            {
                return;
            }
            StateChangeEventHandler handler = _stateChangeEventHandler;
            if (null != handler)
            {
                handler(this, stateChange);
            }
        }

        internal bool ForceNewConnection { get; set; }

        public abstract void Open();

        public Task OpenAsync()
        {
            return OpenAsync(CancellationToken.None);
        }

        public virtual Task OpenAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.SetCanceled();
            }
            else
            {
                try
                {
                    Open();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            }

            return taskCompletionSource.Task;
        }
    }
}
