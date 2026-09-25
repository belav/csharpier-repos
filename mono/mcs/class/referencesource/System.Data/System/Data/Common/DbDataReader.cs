//------------------------------------------------------------------------------
// <copyright file="DbDataReader.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.Common
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DbDataReader : MarshalByRefObject, IDataReader, IEnumerable
    { // V1.2.3300
        protected DbDataReader()
            : base() { }

        public abstract int Depth { get; }

        public abstract int FieldCount { get; }

        public abstract bool HasRows { get; }

        public abstract bool IsClosed { get; }

        public abstract int RecordsAffected { get; }

        public virtual int VisibleFieldCount
        {
            // NOTE: This is virtual because not all providers may choose to support
            //       this property, since it was added in Whidbey
            get { return FieldCount; }
        }

        public abstract object this[int ordinal] { get; }

        public abstract object this[string name] { get; }

        public virtual void Close() { }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public abstract string GetDataTypeName(int ordinal);

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public abstract IEnumerator GetEnumerator();

        public abstract Type GetFieldType(int ordinal);

        public abstract string GetName(int ordinal);

        public abstract int GetOrdinal(string name);

        public virtual DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public abstract bool GetBoolean(int ordinal);

        public abstract byte GetByte(int ordinal);

        public abstract long GetBytes(
            int ordinal,
            long dataOffset,
            byte[] buffer,
            int bufferOffset,
            int length
        );

        public abstract char GetChar(int ordinal);

        public abstract long GetChars(
            int ordinal,
            long dataOffset,
            char[] buffer,
            int bufferOffset,
            int length
        );

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public DbDataReader GetData(int ordinal)
        {
            return GetDbDataReader(ordinal);
        }

        IDataReader IDataRecord.GetData(int ordinal)
        {
            return GetDbDataReader(ordinal);
        }

        protected virtual DbDataReader GetDbDataReader(int ordinal)
        {
            // NOTE: This method is virtual because we're required to implement
            //       it however most providers won't support it. Only the OLE DB
            //       provider supports it right now, and they can override it.
            throw ADP.NotSupported();
        }

        public abstract DateTime GetDateTime(int ordinal);

        public abstract Decimal GetDecimal(int ordinal);

        public abstract double GetDouble(int ordinal);

        public abstract float GetFloat(int ordinal);

        public abstract Guid GetGuid(int ordinal);

        public abstract Int16 GetInt16(int ordinal);

        public abstract Int32 GetInt32(int ordinal);

        public abstract Int64 GetInt64(int ordinal);

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public virtual Type GetProviderSpecificFieldType(int ordinal)
        {
            // NOTE: This is virtual because not all providers may choose to support
            //       this method, since it was added in Whidbey.
            return GetFieldType(ordinal);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public virtual Object GetProviderSpecificValue(int ordinal)
        {
            // NOTE: This is virtual because not all providers may choose to support
            //       this method, since it was added in Whidbey
            return GetValue(ordinal);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public virtual int GetProviderSpecificValues(object[] values)
        {
            // NOTE: This is virtual because not all providers may choose to support
            //       this method, since it was added in Whidbey
            return GetValues(values);
        }

        public abstract String GetString(int ordinal);

        public virtual Stream GetStream(int ordinal)
        {
            using (MemoryStream bufferStream = new MemoryStream())
            {
                long bytesRead = 0;
                long bytesReadTotal = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    bytesRead = GetBytes(ordinal, bytesReadTotal, buffer, 0, buffer.Length);
                    bufferStream.Write(buffer, 0, (int)bytesRead);
                    bytesReadTotal += bytesRead;
                } while (bytesRead > 0);

                return new MemoryStream(bufferStream.ToArray(), false);
            }
        }

        public virtual TextReader GetTextReader(int ordinal)
        {
            if (IsDBNull(ordinal))
            {
                return new StringReader(String.Empty);
            }
            else
            {
                return new StringReader(GetString(ordinal));
            }
        }

        public abstract Object GetValue(int ordinal);

        public virtual T GetFieldValue<T>(int ordinal)
        {
            return (T)GetValue(ordinal);
        }

        public Task<T> GetFieldValueAsync<T>(int ordinal)
        {
            return GetFieldValueAsync<T>(ordinal, CancellationToken.None);
        }

        public virtual Task<T> GetFieldValueAsync<T>(
            int ordinal,
            CancellationToken cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<T>();
            }
            else
            {
                try
                {
                    return Task.FromResult<T>(GetFieldValue<T>(ordinal));
                }
                catch (Exception e)
                {
                    return ADP.CreatedTaskWithException<T>(e);
                }
            }
        }

        public abstract int GetValues(object[] values);

        public abstract bool IsDBNull(int ordinal);

        public Task<bool> IsDBNullAsync(int ordinal)
        {
            return IsDBNullAsync(ordinal, CancellationToken.None);
        }

        public virtual Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return IsDBNull(ordinal) ? ADP.TrueTask : ADP.FalseTask;
                }
                catch (Exception e)
                {
                    return ADP.CreatedTaskWithException<bool>(e);
                }
            }
        }

        public abstract bool NextResult();

        public abstract bool Read();

        public Task<bool> ReadAsync()
        {
            return ReadAsync(CancellationToken.None);
        }

        public virtual Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return Read() ? ADP.TrueTask : ADP.FalseTask;
                }
                catch (Exception e)
                {
                    return ADP.CreatedTaskWithException<bool>(e);
                }
            }
        }

        public Task<bool> NextResultAsync()
        {
            return NextResultAsync(CancellationToken.None);
        }

        public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return ADP.CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return NextResult() ? ADP.TrueTask : ADP.FalseTask;
                }
                catch (Exception e)
                {
                    return ADP.CreatedTaskWithException<bool>(e);
                }
            }
        }
    }
}
