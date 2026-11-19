//------------------------------------------------------------------------------
// <copyright file="DbCommand.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DbCommand : Component, IDbCommand
    { // V1.2.3300
        protected DbCommand()
            : base() { }

        [
            DefaultValue(""),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbCommand_CommandText),
        ]
        public abstract string CommandText { get; set; }

        [
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbCommand_CommandTimeout),
        ]
        public abstract int CommandTimeout { get; set; }

        [
            DefaultValue(System.Data.CommandType.Text),
            RefreshProperties(RefreshProperties.All),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbCommand_CommandType),
        ]
        public abstract CommandType CommandType { get; set; }

        [
            Browsable(false),
            DefaultValue(null),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbCommand_Connection),
        ]
        public DbConnection Connection
        {
            get { return DbConnection; }
            set { DbConnection = value; }
        }

        IDbConnection IDbCommand.Connection
        {
            get { return DbConnection; }
            set { DbConnection = (DbConnection)value; }
        }

        protected abstract DbConnection DbConnection { // V1.2.3300
            get; set; }

        protected abstract DbParameterCollection DbParameterCollection { // V1.2.3300
            get; }

        protected abstract DbTransaction DbTransaction { // V1.2.3300
            get; set; }

        // @devnote: By default, the cmd object is visible on the design surface (i.e. VS7 Server Tray)
        // to limit the number of components that clutter the design surface,
        // when the DataAdapter design wizard generates the insert/update/delete commands it will
        // set the DesignTimeVisible property to false so that cmds won't appear as individual objects
        [
            DefaultValue(true),
            DesignOnly(true),
            Browsable(false),
            EditorBrowsableAttribute(EditorBrowsableState.Never),
        ]
        public abstract bool DesignTimeVisible { get; set; }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            ResCategoryAttribute(Res.DataCategory_Data),
            ResDescriptionAttribute(Res.DbCommand_Parameters),
        ]
        public DbParameterCollection Parameters
        {
            get { return DbParameterCollection; }
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get { return (DbParameterCollection)DbParameterCollection; }
        }

        [
            Browsable(false),
            DefaultValue(null),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            ResDescriptionAttribute(Res.DbCommand_Transaction),
        ]
        public DbTransaction Transaction
        {
            get { return DbTransaction; }
            set { DbTransaction = value; }
        }

        IDbTransaction IDbCommand.Transaction
        {
            get { return DbTransaction; }
            set { DbTransaction = (DbTransaction)value; }
        }

        [
            DefaultValue(System.Data.UpdateRowSource.Both),
            ResCategoryAttribute(Res.DataCategory_Update),
            ResDescriptionAttribute(Res.DbCommand_UpdatedRowSource),
        ]
        public abstract UpdateRowSource UpdatedRowSource { get; set; }

        internal void CancelIgnoreFailure()
        {
            // This method is used to route CancellationTokens to the Cancel method.
            // Cancellation is a suggestion, and exceptions should be ignored
            // rather than allowed to be unhandled, as there is no way to route
            // them to the caller.  It would be expected that the error will be
            // observed anyway from the regular method.  An example is cancelling
            // an operation on a closed connection.
            try
            {
                Cancel();
            }
            catch (Exception) { }
        }

        public abstract void Cancel();

        public DbParameter CreateParameter()
        { // V1.2.3300
            return CreateDbParameter();
        }

        IDbDataParameter IDbCommand.CreateParameter()
        { // V1.2.3300
            return CreateDbParameter();
        }

        protected abstract DbParameter CreateDbParameter();

        protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

        public abstract int ExecuteNonQuery();

        public DbDataReader ExecuteReader()
        {
            return (DbDataReader)ExecuteDbDataReader(CommandBehavior.Default);
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return (DbDataReader)ExecuteDbDataReader(CommandBehavior.Default);
        }

        public DbDataReader ExecuteReader(CommandBehavior behavior)
        {
            return (DbDataReader)ExecuteDbDataReader(behavior);
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return (DbDataReader)ExecuteDbDataReader(behavior);
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            return ExecuteNonQueryAsync(CancellationToken.None);
        }

        public virtual Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<int>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(CancelIgnoreFailure);
                }

                try
                {
                    return Task.FromResult<int>(ExecuteNonQuery());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return ADP.CreatedTaskWithException<int>(e);
                }
            }
        }

        public Task<DbDataReader> ExecuteReaderAsync()
        {
            return ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
        }

        public Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);
        }

        public Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            return ExecuteReaderAsync(behavior, CancellationToken.None);
        }

        public Task<DbDataReader> ExecuteReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken
        )
        {
            return ExecuteDbDataReaderAsync(behavior, cancellationToken);
        }

        protected virtual Task<DbDataReader> ExecuteDbDataReaderAsync(
            CommandBehavior behavior,
            CancellationToken cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<DbDataReader>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(CancelIgnoreFailure);
                }

                try
                {
                    return Task.FromResult<DbDataReader>(ExecuteReader(behavior));
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return ADP.CreatedTaskWithException<DbDataReader>(e);
                }
            }
        }

        public Task<object> ExecuteScalarAsync()
        {
            return ExecuteScalarAsync(CancellationToken.None);
        }

        public virtual Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<object>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(CancelIgnoreFailure);
                }

                try
                {
                    return Task.FromResult<object>(ExecuteScalar());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return ADP.CreatedTaskWithException<object>(e);
                }
            }
        }

        public abstract object ExecuteScalar();

        public abstract void Prepare();
    }
}
