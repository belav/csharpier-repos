//------------------------------------------------------------------------------
// <copyright file="DataException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Serialization;

    // Microsoft: This functions are major point of localization.
    // We need to have a rules to enforce consistency there.
    // The dangerous point there are the string arguments of the exported (internal) methods.
    // This string can be argument, table or constraint name but never text of exception itself.
    // Make an invariant that all texts of exceptions coming from resources only.

    [Serializable]
    public class DataException : SystemException
    {
        protected DataException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public DataException()
            : base(Res.GetString(Res.DataSet_DefaultDataException))
        {
            HResult = HResults.Data;
        }

        public DataException(string s)
            : base(s)
        {
            HResult = HResults.Data;
        }

        public DataException(string s, Exception innerException)
            : base(s, innerException) { }
    };

    [Serializable]
    public class ConstraintException : DataException
    {
        protected ConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public ConstraintException()
            : base(Res.GetString(Res.DataSet_DefaultConstraintException))
        {
            HResult = HResults.DataConstraint;
        }

        public ConstraintException(string s)
            : base(s)
        {
            HResult = HResults.DataConstraint;
        }

        public ConstraintException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataConstraint;
        }
    }

    [Serializable]
    public class DeletedRowInaccessibleException : DataException
    {
        protected DeletedRowInaccessibleException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Data.DeletedRowInaccessibleException'/> class.
        ///    </para>
        /// </devdoc>
        public DeletedRowInaccessibleException()
            : base(Res.GetString(Res.DataSet_DefaultDeletedRowInaccessibleException))
        {
            HResult = HResults.DataDeletedRowInaccessible;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Data.DeletedRowInaccessibleException'/> class with the specified string.
        ///    </para>
        /// </devdoc>
        public DeletedRowInaccessibleException(string s)
            : base(s)
        {
            HResult = HResults.DataDeletedRowInaccessible;
        }

        public DeletedRowInaccessibleException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataDeletedRowInaccessible;
        }
    }

    [Serializable]
    public class DuplicateNameException : DataException
    {
        protected DuplicateNameException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public DuplicateNameException()
            : base(Res.GetString(Res.DataSet_DefaultDuplicateNameException))
        {
            HResult = HResults.DataDuplicateName;
        }

        public DuplicateNameException(string s)
            : base(s)
        {
            HResult = HResults.DataDuplicateName;
        }

        public DuplicateNameException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataDuplicateName;
        }
    }

    [Serializable]
    public class InRowChangingEventException : DataException
    {
        protected InRowChangingEventException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public InRowChangingEventException()
            : base(Res.GetString(Res.DataSet_DefaultInRowChangingEventException))
        {
            HResult = HResults.DataInRowChangingEvent;
        }

        public InRowChangingEventException(string s)
            : base(s)
        {
            HResult = HResults.DataInRowChangingEvent;
        }

        public InRowChangingEventException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataInRowChangingEvent;
        }
    }

    [Serializable]
    public class InvalidConstraintException : DataException
    {
        protected InvalidConstraintException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public InvalidConstraintException()
            : base(Res.GetString(Res.DataSet_DefaultInvalidConstraintException))
        {
            HResult = HResults.DataInvalidConstraint;
        }

        public InvalidConstraintException(string s)
            : base(s)
        {
            HResult = HResults.DataInvalidConstraint;
        }

        public InvalidConstraintException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataInvalidConstraint;
        }
    }

    [Serializable]
    public class MissingPrimaryKeyException : DataException
    {
        protected MissingPrimaryKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public MissingPrimaryKeyException()
            : base(Res.GetString(Res.DataSet_DefaultMissingPrimaryKeyException))
        {
            HResult = HResults.DataMissingPrimaryKey;
        }

        public MissingPrimaryKeyException(string s)
            : base(s)
        {
            HResult = HResults.DataMissingPrimaryKey;
        }

        public MissingPrimaryKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataMissingPrimaryKey;
        }
    }

    [Serializable]
    public class NoNullAllowedException : DataException
    {
        protected NoNullAllowedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public NoNullAllowedException()
            : base(Res.GetString(Res.DataSet_DefaultNoNullAllowedException))
        {
            HResult = HResults.DataNoNullAllowed;
        }

        public NoNullAllowedException(string s)
            : base(s)
        {
            HResult = HResults.DataNoNullAllowed;
        }

        public NoNullAllowedException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataNoNullAllowed;
        }
    }

    [Serializable]
    public class ReadOnlyException : DataException
    {
        protected ReadOnlyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public ReadOnlyException()
            : base(Res.GetString(Res.DataSet_DefaultReadOnlyException))
        {
            HResult = HResults.DataReadOnly;
        }

        public ReadOnlyException(string s)
            : base(s)
        {
            HResult = HResults.DataReadOnly;
        }

        public ReadOnlyException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataReadOnly;
        }
    }

    [Serializable]
    public class RowNotInTableException : DataException
    {
        protected RowNotInTableException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public RowNotInTableException()
            : base(Res.GetString(Res.DataSet_DefaultRowNotInTableException))
        {
            HResult = HResults.DataRowNotInTable;
        }

        public RowNotInTableException(string s)
            : base(s)
        {
            HResult = HResults.DataRowNotInTable;
        }

        public RowNotInTableException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataRowNotInTable;
        }
    }

    [Serializable]
    public class VersionNotFoundException : DataException
    {
        protected VersionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public VersionNotFoundException()
            : base(Res.GetString(Res.DataSet_DefaultVersionNotFoundException))
        {
            HResult = HResults.DataVersionNotFound;
        }

        public VersionNotFoundException(string s)
            : base(s)
        {
            HResult = HResults.DataVersionNotFound;
        }

        public VersionNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.DataVersionNotFound;
        }
    }

    internal static class ExceptionBuilder
    {
        // The class defines the exceptions that are specific to the DataSet.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource Data.txt.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error messages.
        // The resource Data.txt will ensure proper string text based on the appropriate
        // locale.

        [BidMethod] // this method accepts BID format as an argument, this attribute allows FXCopBid rule to validate calls to it
        private static void TraceException(
            string trace,
            [BidArgumentType(typeof(String))] Exception e
        )
        {
            Debug.Assert(null != e, "TraceException: null Exception");
            if (null != e)
            {
                Bid.Trace(trace, e.Message);
                if (Bid.AdvancedOn)
                {
                    try
                    {
                        Bid.Trace(", StackTrace='%ls'", Environment.StackTrace);
                    }
                    catch (System.Security.SecurityException)
                    {
                        // if you don't have permission - you don't get the stack trace
                    }
                }
                Bid.Trace("\n");
            }
        }

        internal static void TraceExceptionAsReturnValue(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|THROW> Message='%ls'", e);
        }

        internal static void TraceExceptionForCapture(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|CATCH> Message='%ls'", e);
        }

        internal static void TraceExceptionWithoutRethrow(Exception e)
        {
            TraceException("<comm.ADP.TraceException|ERR|CATCH> Message='%ls'", e);
        }

        //
        // COM+ exceptions
        //
        static internal ArgumentException _Argument(string error)
        {
            ArgumentException e = new ArgumentException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException _Argument(string paramName, string error)
        {
            ArgumentException e = new ArgumentException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentException _Argument(string error, Exception innerException)
        {
            ArgumentException e = new ArgumentException(error, innerException);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static ArgumentNullException _ArgumentNull(string paramName, string msg)
        {
            ArgumentNullException e = new ArgumentNullException(paramName, msg);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static ArgumentOutOfRangeException _ArgumentOutOfRange(
            string paramName,
            string msg
        )
        {
            ArgumentOutOfRangeException e = new ArgumentOutOfRangeException(paramName, msg);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static IndexOutOfRangeException _IndexOutOfRange(string error)
        {
            IndexOutOfRangeException e = new IndexOutOfRangeException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidOperationException _InvalidOperation(string error)
        {
            InvalidOperationException e = new InvalidOperationException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidEnumArgumentException _InvalidEnumArgumentException(string error)
        {
            InvalidEnumArgumentException e = new InvalidEnumArgumentException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidEnumArgumentException _InvalidEnumArgumentException<T>(T value)
        {
            string msg = Res.GetString(
                Res.ADP_InvalidEnumerationValue,
                typeof(T).Name,
                value.ToString()
            );
            return _InvalidEnumArgumentException(msg);
        }

        //
        // System.Data exceptions
        //
        static private DataException _Data(string error)
        {
            DataException e = new DataException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        /// <summary>trace and throw a DataException</summary>
        /// <param name="error">exception Message</param>
        /// <param name="innerException">exception InnerException</param>
        /// <exception cref="DataException">always thrown</exception>
        static private void ThrowDataException(string error, Exception innerException)
        {
            DataException e = new DataException(error, innerException);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            throw e;
        }

        private static ConstraintException _Constraint(string error)
        {
            ConstraintException e = new ConstraintException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidConstraintException _InvalidConstraint(string error)
        {
            InvalidConstraintException e = new InvalidConstraintException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static DeletedRowInaccessibleException _DeletedRowInaccessible(string error)
        {
            DeletedRowInaccessibleException e = new DeletedRowInaccessibleException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static DuplicateNameException _DuplicateName(string error)
        {
            DuplicateNameException e = new DuplicateNameException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InRowChangingEventException _InRowChangingEvent(string error)
        {
            InRowChangingEventException e = new InRowChangingEventException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static MissingPrimaryKeyException _MissingPrimaryKey(string error)
        {
            MissingPrimaryKeyException e = new MissingPrimaryKeyException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static NoNullAllowedException _NoNullAllowed(string error)
        {
            NoNullAllowedException e = new NoNullAllowedException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static ReadOnlyException _ReadOnly(string error)
        {
            ReadOnlyException e = new ReadOnlyException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static RowNotInTableException _RowNotInTable(string error)
        {
            RowNotInTableException e = new RowNotInTableException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static VersionNotFoundException _VersionNotFound(string error)
        {
            VersionNotFoundException e = new VersionNotFoundException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        // Consider: whether we need to keep our own texts from Data_ArgumentNull and Data_ArgumentOutOfRange?
        // Unfortunately ours and the system ones are not consisten between each other. Try to raise this isue in "URT user comunity"
        static public Exception ArgumentNull(string paramName)
        {
            return _ArgumentNull(paramName, Res.GetString(Res.Data_ArgumentNull, paramName));
        }

        public static Exception ArgumentOutOfRange(string paramName)
        {
            return _ArgumentOutOfRange(
                paramName,
                Res.GetString(Res.Data_ArgumentOutOfRange, paramName)
            );
        }

        public static Exception BadObjectPropertyAccess(string error)
        {
            return _InvalidOperation(
                Res.GetString(Res.DataConstraint_BadObjectPropertyAccess, error)
            );
        }

        public static Exception ArgumentContainsNull(string paramName)
        {
            return _Argument(paramName, Res.GetString(Res.Data_ArgumentContainsNull, paramName));
        }

        //
        // Collections
        //

        static public Exception CannotModifyCollection()
        {
            return _Argument(Res.GetString(Res.Data_CannotModifyCollection));
        }

        public static Exception CaseInsensitiveNameConflict(string name)
        {
            return _Argument(Res.GetString(Res.Data_CaseInsensitiveNameConflict, name));
        }

        public static Exception NamespaceNameConflict(string name)
        {
            return _Argument(Res.GetString(Res.Data_NamespaceNameConflict, name));
        }

        public static Exception InvalidOffsetLength()
        {
            return _Argument(Res.GetString(Res.Data_InvalidOffsetLength));
        }

        //
        // DataColumnCollection
        //

        static public Exception ColumnNotInTheTable(string column, string table)
        {
            return _Argument(Res.GetString(Res.DataColumn_NotInTheTable, column, table));
        }

        public static Exception ColumnNotInAnyTable()
        {
            return _Argument(Res.GetString(Res.DataColumn_NotInAnyTable));
        }

        public static Exception ColumnOutOfRange(int index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataColumns_OutOfRange,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception ColumnOutOfRange(string column)
        {
            return _IndexOutOfRange(Res.GetString(Res.DataColumns_OutOfRange, column));
        }

        public static Exception CannotAddColumn1(string column)
        {
            return _Argument(Res.GetString(Res.DataColumns_Add1, column));
        }

        public static Exception CannotAddColumn2(string column)
        {
            return _Argument(Res.GetString(Res.DataColumns_Add2, column));
        }

        public static Exception CannotAddColumn3()
        {
            return _Argument(Res.GetString(Res.DataColumns_Add3));
        }

        public static Exception CannotAddColumn4(string column)
        {
            return _Argument(Res.GetString(Res.DataColumns_Add4, column));
        }

        public static Exception CannotAddDuplicate(string column)
        {
            return _DuplicateName(Res.GetString(Res.DataColumns_AddDuplicate, column));
        }

        public static Exception CannotAddDuplicate2(string table)
        {
            return _DuplicateName(Res.GetString(Res.DataColumns_AddDuplicate2, table));
        }

        public static Exception CannotAddDuplicate3(string table)
        {
            return _DuplicateName(Res.GetString(Res.DataColumns_AddDuplicate3, table));
        }

        public static Exception CannotRemoveColumn()
        {
            return _Argument(Res.GetString(Res.DataColumns_Remove));
        }

        public static Exception CannotRemovePrimaryKey()
        {
            return _Argument(Res.GetString(Res.DataColumns_RemovePrimaryKey));
        }

        public static Exception CannotRemoveChildKey(string relation)
        {
            return _Argument(Res.GetString(Res.DataColumns_RemoveChildKey, relation));
        }

        public static Exception CannotRemoveConstraint(string constraint, string table)
        {
            return _Argument(Res.GetString(Res.DataColumns_RemoveConstraint, constraint, table));
        }

        public static Exception CannotRemoveExpression(string column, string expression)
        {
            return _Argument(Res.GetString(Res.DataColumns_RemoveExpression, column, expression));
        }

        public static Exception ColumnNotInTheUnderlyingTable(string column, string table)
        {
            return _Argument(Res.GetString(Res.DataColumn_NotInTheUnderlyingTable, column, table));
        }

        public static Exception InvalidOrdinal(string name, int ordinal)
        {
            return _ArgumentOutOfRange(
                name,
                Res.GetString(
                    Res.DataColumn_OrdinalExceedMaximun,
                    (ordinal).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        //
        // _Constraint and ConstrainsCollection
        //

        static public Exception AddPrimaryKeyConstraint()
        {
            return _Argument(Res.GetString(Res.DataConstraint_AddPrimaryKeyConstraint));
        }

        public static Exception NoConstraintName()
        {
            return _Argument(Res.GetString(Res.DataConstraint_NoName));
        }

        public static Exception ConstraintViolation(string constraint)
        {
            return _Constraint(Res.GetString(Res.DataConstraint_Violation, constraint));
        }

        public static Exception ConstraintNotInTheTable(string constraint)
        {
            return _Argument(Res.GetString(Res.DataConstraint_NotInTheTable, constraint));
        }

        public static string KeysToString(object[] keys)
        {
            string values = String.Empty;
            for (int i = 0; i < keys.Length; i++)
            {
                values +=
                    Convert.ToString(keys[i], null) + (i < keys.Length - 1 ? ", " : String.Empty);
            }
            return values;
        }

        public static string UniqueConstraintViolationText(DataColumn[] columns, object[] values)
        {
            if (columns.Length > 1)
            {
                string columnNames = String.Empty;
                for (int i = 0; i < columns.Length; i++)
                {
                    columnNames += columns[i].ColumnName + (i < columns.Length - 1 ? ", " : "");
                }
                return Res.GetString(
                    Res.DataConstraint_ViolationValue,
                    columnNames,
                    KeysToString(values)
                );
            }
            else
            {
                return Res.GetString(
                    Res.DataConstraint_ViolationValue,
                    columns[0].ColumnName,
                    Convert.ToString(values[0], null)
                );
            }
        }

        public static Exception ConstraintViolation(DataColumn[] columns, object[] values)
        {
            return _Constraint(UniqueConstraintViolationText(columns, values));
        }

        public static Exception ConstraintOutOfRange(int index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataConstraint_OutOfRange,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception DuplicateConstraint(string constraint)
        {
            return _Data(Res.GetString(Res.DataConstraint_Duplicate, constraint));
        }

        public static Exception DuplicateConstraintName(string constraint)
        {
            return _DuplicateName(Res.GetString(Res.DataConstraint_DuplicateName, constraint));
        }

        public static Exception NeededForForeignKeyConstraint(
            UniqueConstraint key,
            ForeignKeyConstraint fk
        )
        {
            return _Argument(
                Res.GetString(
                    Res.DataConstraint_NeededForForeignKeyConstraint,
                    key.ConstraintName,
                    fk.ConstraintName
                )
            );
        }

        public static Exception UniqueConstraintViolation()
        {
            return _Argument(Res.GetString(Res.DataConstraint_UniqueViolation));
        }

        public static Exception ConstraintForeignTable()
        {
            return _Argument(Res.GetString(Res.DataConstraint_ForeignTable));
        }

        public static Exception ConstraintParentValues()
        {
            return _Argument(Res.GetString(Res.DataConstraint_ParentValues));
        }

        public static Exception ConstraintAddFailed(DataTable table)
        {
            return _InvalidConstraint(Res.GetString(Res.DataConstraint_AddFailed, table.TableName));
        }

        public static Exception ConstraintRemoveFailed()
        {
            return _Argument(Res.GetString(Res.DataConstraint_RemoveFailed));
        }

        public static Exception FailedCascadeDelete(string constraint)
        {
            return _InvalidConstraint(Res.GetString(Res.DataConstraint_CascadeDelete, constraint));
        }

        public static Exception FailedCascadeUpdate(string constraint)
        {
            return _InvalidConstraint(Res.GetString(Res.DataConstraint_CascadeUpdate, constraint));
        }

        public static Exception FailedClearParentTable(
            string table,
            string constraint,
            string childTable
        )
        {
            return _InvalidConstraint(
                Res.GetString(Res.DataConstraint_ClearParentTable, table, constraint, childTable)
            );
        }

        public static Exception ForeignKeyViolation(string constraint, object[] keys)
        {
            return _InvalidConstraint(
                Res.GetString(
                    Res.DataConstraint_ForeignKeyViolation,
                    constraint,
                    KeysToString(keys)
                )
            );
        }

        public static Exception RemoveParentRow(ForeignKeyConstraint constraint)
        {
            return _InvalidConstraint(
                Res.GetString(Res.DataConstraint_RemoveParentRow, constraint.ConstraintName)
            );
        }

        public static string MaxLengthViolationText(string columnName)
        {
            return Res.GetString(Res.DataColumn_ExceedMaxLength, columnName);
        }

        public static string NotAllowDBNullViolationText(string columnName)
        {
            return Res.GetString(Res.DataColumn_NotAllowDBNull, columnName);
        }

        public static Exception CantAddConstraintToMultipleNestedTable(string tableName)
        {
            return _Argument(
                Res.GetString(Res.DataConstraint_CantAddConstraintToMultipleNestedTable, tableName)
            );
        }

        //
        // DataColumn Set Properties conflicts
        //

        static public Exception AutoIncrementAndExpression()
        {
            return _Argument(Res.GetString(Res.DataColumn_AutoIncrementAndExpression));
        }

        public static Exception AutoIncrementAndDefaultValue()
        {
            return _Argument(Res.GetString(Res.DataColumn_AutoIncrementAndDefaultValue));
        }

        public static Exception AutoIncrementSeed()
        {
            return _Argument(Res.GetString(Res.DataColumn_AutoIncrementSeed));
        }

        public static Exception CantChangeDataType()
        {
            return _Argument(Res.GetString(Res.DataColumn_ChangeDataType));
        }

        public static Exception NullDataType()
        {
            return _Argument(Res.GetString(Res.DataColumn_NullDataType));
        }

        public static Exception ColumnNameRequired()
        {
            return _Argument(Res.GetString(Res.DataColumn_NameRequired));
        }

        public static Exception DefaultValueAndAutoIncrement()
        {
            return _Argument(Res.GetString(Res.DataColumn_DefaultValueAndAutoIncrement));
        }

        public static Exception DefaultValueDataType(
            string column,
            Type defaultType,
            Type columnType,
            Exception inner
        )
        {
            if (column.Length == 0)
            {
                return _Argument(
                    Res.GetString(
                        Res.DataColumn_DefaultValueDataType1,
                        defaultType.FullName,
                        columnType.FullName
                    ),
                    inner
                );
            }
            else
            {
                return _Argument(
                    Res.GetString(
                        Res.DataColumn_DefaultValueDataType,
                        column,
                        defaultType.FullName,
                        columnType.FullName
                    ),
                    inner
                );
            }
        }

        public static Exception DefaultValueColumnDataType(
            string column,
            Type defaultType,
            Type columnType,
            Exception inner
        )
        {
            return _Argument(
                Res.GetString(
                    Res.DataColumn_DefaultValueColumnDataType,
                    column,
                    defaultType.FullName,
                    columnType.FullName
                ),
                inner
            );
        }

        public static Exception ExpressionAndUnique()
        {
            return _Argument(Res.GetString(Res.DataColumn_ExpressionAndUnique));
        }

        public static Exception ExpressionAndReadOnly()
        {
            return _Argument(Res.GetString(Res.DataColumn_ExpressionAndReadOnly));
        }

        public static Exception ExpressionAndConstraint(DataColumn column, Constraint constraint)
        {
            return _Argument(
                Res.GetString(
                    Res.DataColumn_ExpressionAndConstraint,
                    column.ColumnName,
                    constraint.ConstraintName
                )
            );
        }

        public static Exception ExpressionInConstraint(DataColumn column)
        {
            return _Argument(
                Res.GetString(Res.DataColumn_ExpressionInConstraint, column.ColumnName)
            );
        }

        public static Exception ExpressionCircular()
        {
            return _Argument(Res.GetString(Res.DataColumn_ExpressionCircular));
        }

        public static Exception NonUniqueValues(string column)
        {
            return _InvalidConstraint(Res.GetString(Res.DataColumn_NonUniqueValues, column));
        }

        public static Exception NullKeyValues(string column)
        {
            return _Data(Res.GetString(Res.DataColumn_NullKeyValues, column));
        }

        public static Exception NullValues(string column)
        {
            return _NoNullAllowed(Res.GetString(Res.DataColumn_NullValues, column));
        }

        public static Exception ReadOnlyAndExpression()
        {
            return _ReadOnly(Res.GetString(Res.DataColumn_ReadOnlyAndExpression));
        }

        public static Exception ReadOnly(string column)
        {
            return _ReadOnly(Res.GetString(Res.DataColumn_ReadOnly, column));
        }

        public static Exception UniqueAndExpression()
        {
            return _Argument(Res.GetString(Res.DataColumn_UniqueAndExpression));
        }

        public static Exception SetFailed(
            object value,
            DataColumn column,
            Type type,
            Exception innerException
        )
        {
            return _Argument(
                innerException.Message
                    + Res.GetString(
                        Res.DataColumn_SetFailed,
                        value.ToString(),
                        column.ColumnName,
                        type.Name
                    ),
                innerException
            );
        }

        public static Exception CannotSetToNull(DataColumn column)
        {
            return _Argument(Res.GetString(Res.DataColumn_CannotSetToNull, column.ColumnName));
        }

        public static Exception LongerThanMaxLength(DataColumn column)
        {
            return _Argument(Res.GetString(Res.DataColumn_LongerThanMaxLength, column.ColumnName));
        }

        public static Exception CannotSetMaxLength(DataColumn column, int value)
        {
            return _Argument(
                Res.GetString(
                    Res.DataColumn_CannotSetMaxLength,
                    column.ColumnName,
                    value.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception CannotSetMaxLength2(DataColumn column)
        {
            return _Argument(Res.GetString(Res.DataColumn_CannotSetMaxLength2, column.ColumnName));
        }

        public static Exception CannotSetSimpleContentType(String columnName, Type type)
        {
            return _Argument(
                Res.GetString(Res.DataColumn_CannotSimpleContentType, columnName, type)
            );
        }

        public static Exception CannotSetSimpleContent(String columnName, Type type)
        {
            return _Argument(Res.GetString(Res.DataColumn_CannotSimpleContent, columnName, type));
        }

        public static Exception CannotChangeNamespace(String columnName)
        {
            return _Argument(Res.GetString(Res.DataColumn_CannotChangeNamespace, columnName));
        }

        public static Exception HasToBeStringType(DataColumn column)
        {
            return _Argument(Res.GetString(Res.DataColumn_HasToBeStringType, column.ColumnName));
        }

        public static Exception AutoIncrementCannotSetIfHasData(string typeName)
        {
            return _Argument(
                Res.GetString(Res.DataColumn_AutoIncrementCannotSetIfHasData, typeName)
            );
        }

        public static Exception INullableUDTwithoutStaticNull(string typeName)
        {
            return _Argument(Res.GetString(Res.DataColumn_INullableUDTwithoutStaticNull, typeName));
        }

        public static Exception IComparableNotImplemented(string typeName)
        {
            return _Data(Res.GetString(Res.DataStorage_IComparableNotDefined, typeName));
        }

        public static Exception UDTImplementsIChangeTrackingButnotIRevertible(string typeName)
        {
            return _InvalidOperation(
                Res.GetString(
                    Res.DataColumn_UDTImplementsIChangeTrackingButnotIRevertible,
                    typeName
                )
            );
        }

        public static Exception SetAddedAndModifiedCalledOnnonUnchanged()
        {
            return _InvalidOperation(
                Res.GetString(Res.DataColumn_SetAddedAndModifiedCalledOnNonUnchanged)
            );
        }

        public static Exception InvalidDataColumnMapping(Type type)
        {
            return _Argument(
                Res.GetString(Res.DataColumn_InvalidDataColumnMapping, type.AssemblyQualifiedName)
            );
        }

        public static Exception CannotSetDateTimeModeForNonDateTimeColumns()
        {
            return _InvalidOperation(
                Res.GetString(Res.DataColumn_CannotSetDateTimeModeForNonDateTimeColumns)
            );
        }

        public static Exception InvalidDateTimeMode(DataSetDateTime mode)
        {
            return _InvalidEnumArgumentException<DataSetDateTime>(mode);
        }

        public static Exception CantChangeDateTimeMode(
            DataSetDateTime oldValue,
            DataSetDateTime newValue
        )
        {
            return _InvalidOperation(
                Res.GetString(Res.DataColumn_DateTimeMode, oldValue.ToString(), newValue.ToString())
            );
        }

        public static Exception ColumnTypeNotSupported()
        {
            return System
                .Data
                .Common
                .ADP
                .NotSupported(Res.GetString(Res.DataColumn_NullableTypesNotSupported));
        }

        //
        // DataView
        //

        static public Exception SetFailed(string name)
        {
            return _Data(Res.GetString(Res.DataView_SetFailed, name));
        }

        public static Exception SetDataSetFailed()
        {
            return _Data(Res.GetString(Res.DataView_SetDataSetFailed));
        }

        public static Exception SetRowStateFilter()
        {
            return _Data(Res.GetString(Res.DataView_SetRowStateFilter));
        }

        public static Exception CanNotSetDataSet()
        {
            return _Data(Res.GetString(Res.DataView_CanNotSetDataSet));
        }

        public static Exception CanNotUseDataViewManager()
        {
            return _Data(Res.GetString(Res.DataView_CanNotUseDataViewManager));
        }

        public static Exception CanNotSetTable()
        {
            return _Data(Res.GetString(Res.DataView_CanNotSetTable));
        }

        public static Exception CanNotUse()
        {
            return _Data(Res.GetString(Res.DataView_CanNotUse));
        }

        public static Exception CanNotBindTable()
        {
            return _Data(Res.GetString(Res.DataView_CanNotBindTable));
        }

        public static Exception SetTable()
        {
            return _Data(Res.GetString(Res.DataView_SetTable));
        }

        public static Exception SetIListObject()
        {
            return _Argument(Res.GetString(Res.DataView_SetIListObject));
        }

        public static Exception AddNewNotAllowNull()
        {
            return _Data(Res.GetString(Res.DataView_AddNewNotAllowNull));
        }

        public static Exception NotOpen()
        {
            return _Data(Res.GetString(Res.DataView_NotOpen));
        }

        public static Exception CreateChildView()
        {
            return _Argument(Res.GetString(Res.DataView_CreateChildView));
        }

        public static Exception CanNotDelete()
        {
            return _Data(Res.GetString(Res.DataView_CanNotDelete));
        }

        public static Exception CanNotEdit()
        {
            return _Data(Res.GetString(Res.DataView_CanNotEdit));
        }

        public static Exception GetElementIndex(Int32 index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataView_GetElementIndex,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception AddExternalObject()
        {
            return _Argument(Res.GetString(Res.DataView_AddExternalObject));
        }

        public static Exception CanNotClear()
        {
            return _Argument(Res.GetString(Res.DataView_CanNotClear));
        }

        public static Exception InsertExternalObject()
        {
            return _Argument(Res.GetString(Res.DataView_InsertExternalObject));
        }

        public static Exception RemoveExternalObject()
        {
            return _Argument(Res.GetString(Res.DataView_RemoveExternalObject));
        }

        public static Exception PropertyNotFound(string property, string table)
        {
            return _Argument(Res.GetString(Res.DataROWView_PropertyNotFound, property, table));
        }

        public static Exception ColumnToSortIsOutOfRange(string column)
        {
            return _Argument(Res.GetString(Res.DataColumns_OutOfRange, column));
        }

        //
        // Keys
        //

        static public Exception KeyTableMismatch()
        {
            return _InvalidConstraint(Res.GetString(Res.DataKey_TableMismatch));
        }

        public static Exception KeyNoColumns()
        {
            return _InvalidConstraint(Res.GetString(Res.DataKey_NoColumns));
        }

        public static Exception KeyTooManyColumns(int cols)
        {
            return _InvalidConstraint(
                Res.GetString(
                    Res.DataKey_TooManyColumns,
                    (cols).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception KeyDuplicateColumns(string columnName)
        {
            return _InvalidConstraint(Res.GetString(Res.DataKey_DuplicateColumns, columnName));
        }

        //
        // Relations, constraints
        //

        static public Exception RelationDataSetMismatch()
        {
            return _InvalidConstraint(Res.GetString(Res.DataRelation_DataSetMismatch));
        }

        public static Exception NoRelationName()
        {
            return _Argument(Res.GetString(Res.DataRelation_NoName));
        }

        public static Exception ColumnsTypeMismatch()
        {
            return _InvalidConstraint(Res.GetString(Res.DataRelation_ColumnsTypeMismatch));
        }

        public static Exception KeyLengthMismatch()
        {
            return _Argument(Res.GetString(Res.DataRelation_KeyLengthMismatch));
        }

        public static Exception KeyLengthZero()
        {
            return _Argument(Res.GetString(Res.DataRelation_KeyZeroLength));
        }

        public static Exception ForeignRelation()
        {
            return _Argument(Res.GetString(Res.DataRelation_ForeignDataSet));
        }

        public static Exception KeyColumnsIdentical()
        {
            return _InvalidConstraint(Res.GetString(Res.DataRelation_KeyColumnsIdentical));
        }

        public static Exception RelationForeignTable(string t1, string t2)
        {
            return _InvalidConstraint(Res.GetString(Res.DataRelation_ForeignTable, t1, t2));
        }

        public static Exception GetParentRowTableMismatch(string t1, string t2)
        {
            return _InvalidConstraint(
                Res.GetString(Res.DataRelation_GetParentRowTableMismatch, t1, t2)
            );
        }

        public static Exception SetParentRowTableMismatch(string t1, string t2)
        {
            return _InvalidConstraint(
                Res.GetString(Res.DataRelation_SetParentRowTableMismatch, t1, t2)
            );
        }

        public static Exception RelationForeignRow()
        {
            return _Argument(Res.GetString(Res.DataRelation_ForeignRow));
        }

        public static Exception RelationNestedReadOnly()
        {
            return _Argument(Res.GetString(Res.DataRelation_RelationNestedReadOnly));
        }

        public static Exception TableCantBeNestedInTwoTables(string tableName)
        {
            return _Argument(
                Res.GetString(Res.DataRelation_TableCantBeNestedInTwoTables, tableName)
            );
        }

        public static Exception LoopInNestedRelations(string tableName)
        {
            return _Argument(Res.GetString(Res.DataRelation_LoopInNestedRelations, tableName));
        }

        public static Exception RelationDoesNotExist()
        {
            return _Argument(Res.GetString(Res.DataRelation_DoesNotExist));
        }

        public static Exception ParentRowNotInTheDataSet()
        {
            return _Argument(Res.GetString(Res.DataRow_ParentRowNotInTheDataSet));
        }

        public static Exception ParentOrChildColumnsDoNotHaveDataSet()
        {
            return _InvalidConstraint(
                Res.GetString(Res.DataRelation_ParentOrChildColumnsDoNotHaveDataSet)
            );
        }

        public static Exception InValidNestedRelation(string childTableName)
        {
            return _InvalidOperation(
                Res.GetString(Res.DataRelation_InValidNestedRelation, childTableName)
            );
        }

        public static Exception InvalidParentNamespaceinNestedRelation(string childTableName)
        {
            return _InvalidOperation(
                Res.GetString(Res.DataRelation_InValidNamespaceInNestedRelation, childTableName)
            );
        }

        //
        // Rows
        //

        static public Exception RowNotInTheDataSet()
        {
            return _Argument(Res.GetString(Res.DataRow_NotInTheDataSet));
        }

        public static Exception RowNotInTheTable()
        {
            return _RowNotInTable(Res.GetString(Res.DataRow_NotInTheTable));
        }

        public static Exception EditInRowChanging()
        {
            return _InRowChangingEvent(Res.GetString(Res.DataRow_EditInRowChanging));
        }

        public static Exception EndEditInRowChanging()
        {
            return _InRowChangingEvent(Res.GetString(Res.DataRow_EndEditInRowChanging));
        }

        public static Exception BeginEditInRowChanging()
        {
            return _InRowChangingEvent(Res.GetString(Res.DataRow_BeginEditInRowChanging));
        }

        public static Exception CancelEditInRowChanging()
        {
            return _InRowChangingEvent(Res.GetString(Res.DataRow_CancelEditInRowChanging));
        }

        public static Exception DeleteInRowDeleting()
        {
            return _InRowChangingEvent(Res.GetString(Res.DataRow_DeleteInRowDeleting));
        }

        public static Exception ValueArrayLength()
        {
            return _Argument(Res.GetString(Res.DataRow_ValuesArrayLength));
        }

        public static Exception NoCurrentData()
        {
            return _VersionNotFound(Res.GetString(Res.DataRow_NoCurrentData));
        }

        public static Exception NoOriginalData()
        {
            return _VersionNotFound(Res.GetString(Res.DataRow_NoOriginalData));
        }

        public static Exception NoProposedData()
        {
            return _VersionNotFound(Res.GetString(Res.DataRow_NoProposedData));
        }

        public static Exception RowRemovedFromTheTable()
        {
            return _RowNotInTable(Res.GetString(Res.DataRow_RemovedFromTheTable));
        }

        public static Exception DeletedRowInaccessible()
        {
            return _DeletedRowInaccessible(Res.GetString(Res.DataRow_DeletedRowInaccessible));
        }

        public static Exception RowAlreadyDeleted()
        {
            return _DeletedRowInaccessible(Res.GetString(Res.DataRow_AlreadyDeleted));
        }

        public static Exception RowEmpty()
        {
            return _Argument(Res.GetString(Res.DataRow_Empty));
        }

        public static Exception InvalidRowVersion()
        {
            return _Data(Res.GetString(Res.DataRow_InvalidVersion));
        }

        public static Exception RowOutOfRange()
        {
            return _IndexOutOfRange(Res.GetString(Res.DataRow_RowOutOfRange));
        }

        public static Exception RowOutOfRange(int index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataRow_OutOfRange,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception RowInsertOutOfRange(int index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataRow_RowInsertOutOfRange,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception RowInsertTwice(int index, string tableName)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataRow_RowInsertTwice,
                    (index).ToString(CultureInfo.InvariantCulture),
                    tableName
                )
            );
        }

        public static Exception RowInsertMissing(string tableName)
        {
            return _IndexOutOfRange(Res.GetString(Res.DataRow_RowInsertMissing, tableName));
        }

        public static Exception RowAlreadyRemoved()
        {
            return _Data(Res.GetString(Res.DataRow_AlreadyRemoved));
        }

        public static Exception MultipleParents()
        {
            return _Data(Res.GetString(Res.DataRow_MultipleParents));
        }

        public static Exception InvalidRowState(DataRowState state)
        {
            return _InvalidEnumArgumentException<DataRowState>(state);
        }

        public static Exception InvalidRowBitPattern()
        {
            return _Argument(Res.GetString(Res.DataRow_InvalidRowBitPattern));
        }

        //
        // DataSet
        //

        static internal Exception SetDataSetNameToEmpty()
        {
            return _Argument(Res.GetString(Res.DataSet_SetNameToEmpty));
        }

        internal static Exception SetDataSetNameConflicting(string name)
        {
            return _Argument(Res.GetString(Res.DataSet_SetDataSetNameConflicting, name));
        }

        public static Exception DataSetUnsupportedSchema(string ns)
        {
            return _Argument(Res.GetString(Res.DataSet_UnsupportedSchema, ns));
        }

        public static Exception MergeMissingDefinition(string obj)
        {
            return _Argument(Res.GetString(Res.DataMerge_MissingDefinition, obj));
        }

        public static Exception TablesInDifferentSets()
        {
            return _Argument(Res.GetString(Res.DataRelation_TablesInDifferentSets));
        }

        public static Exception RelationAlreadyExists()
        {
            return _Argument(Res.GetString(Res.DataRelation_AlreadyExists));
        }

        public static Exception RowAlreadyInOtherCollection()
        {
            return _Argument(Res.GetString(Res.DataRow_AlreadyInOtherCollection));
        }

        public static Exception RowAlreadyInTheCollection()
        {
            return _Argument(Res.GetString(Res.DataRow_AlreadyInTheCollection));
        }

        public static Exception TableMissingPrimaryKey()
        {
            return _MissingPrimaryKey(Res.GetString(Res.DataTable_MissingPrimaryKey));
        }

        public static Exception RecordStateRange()
        {
            return _Argument(Res.GetString(Res.DataIndex_RecordStateRange));
        }

        public static Exception IndexKeyLength(int length, int keyLength)
        {
            if (length == 0)
            {
                return _Argument(Res.GetString(Res.DataIndex_FindWithoutSortOrder));
            }
            else
            {
                return _Argument(
                    Res.GetString(
                        Res.DataIndex_KeyLength,
                        (length).ToString(CultureInfo.InvariantCulture),
                        (keyLength).ToString(CultureInfo.InvariantCulture)
                    )
                );
            }
        }

        public static Exception RemovePrimaryKey(DataTable table)
        {
            if (table.TableName.Length == 0)
            {
                return _Argument(Res.GetString(Res.DataKey_RemovePrimaryKey));
            }
            else
            {
                return _Argument(Res.GetString(Res.DataKey_RemovePrimaryKey1, table.TableName));
            }
        }

        public static Exception RelationAlreadyInOtherDataSet()
        {
            return _Argument(Res.GetString(Res.DataRelation_AlreadyInOtherDataSet));
        }

        public static Exception RelationAlreadyInTheDataSet()
        {
            return _Argument(Res.GetString(Res.DataRelation_AlreadyInTheDataSet));
        }

        public static Exception RelationNotInTheDataSet(string relation)
        {
            return _Argument(Res.GetString(Res.DataRelation_NotInTheDataSet, relation));
        }

        public static Exception RelationOutOfRange(object index)
        {
            return _IndexOutOfRange(
                Res.GetString(Res.DataRelation_OutOfRange, Convert.ToString(index, null))
            );
        }

        public static Exception DuplicateRelation(string relation)
        {
            return _DuplicateName(Res.GetString(Res.DataRelation_DuplicateName, relation));
        }

        public static Exception RelationTableNull()
        {
            return _Argument(Res.GetString(Res.DataRelation_TableNull));
        }

        public static Exception RelationDataSetNull()
        {
            return _Argument(Res.GetString(Res.DataRelation_TableNull));
        }

        public static Exception RelationTableWasRemoved()
        {
            return _Argument(Res.GetString(Res.DataRelation_TableWasRemoved));
        }

        public static Exception ParentTableMismatch()
        {
            return _Argument(Res.GetString(Res.DataRelation_ParentTableMismatch));
        }

        public static Exception ChildTableMismatch()
        {
            return _Argument(Res.GetString(Res.DataRelation_ChildTableMismatch));
        }

        public static Exception EnforceConstraint()
        {
            return _Constraint(Res.GetString(Res.Data_EnforceConstraints));
        }

        public static Exception CaseLocaleMismatch()
        {
            return _Argument(Res.GetString(Res.DataRelation_CaseLocaleMismatch));
        }

        public static Exception CannotChangeCaseLocale()
        {
            return CannotChangeCaseLocale(null);
        }

        public static Exception CannotChangeCaseLocale(Exception innerException)
        {
            return _Argument(Res.GetString(Res.DataSet_CannotChangeCaseLocale), innerException);
        }

        public static Exception CannotChangeSchemaSerializationMode()
        {
            return _InvalidOperation(
                Res.GetString(Res.DataSet_CannotChangeSchemaSerializationMode)
            );
        }

        public static Exception InvalidSchemaSerializationMode(Type enumType, string mode)
        {
            return _InvalidEnumArgumentException(
                Res.GetString(Res.ADP_InvalidEnumerationValue, enumType.Name, mode)
            );
        }

        public static Exception InvalidRemotingFormat(SerializationFormat mode)
        {
#if DEBUG
            switch (mode)
            {
                case SerializationFormat.Xml:
                case SerializationFormat.Binary:
                    Debug.Assert(false, "valid SerializationFormat " + mode.ToString());
                    break;
            }
#endif
            return _InvalidEnumArgumentException<SerializationFormat>(mode);
        }

        //
        // DataTable and DataTableCollection
        //
        static public Exception TableForeignPrimaryKey()
        {
            return _Argument(Res.GetString(Res.DataTable_ForeignPrimaryKey));
        }

        public static Exception TableCannotAddToSimpleContent()
        {
            return _Argument(Res.GetString(Res.DataTable_CannotAddToSimpleContent));
        }

        public static Exception NoTableName()
        {
            return _Argument(Res.GetString(Res.DataTable_NoName));
        }

        public static Exception MultipleTextOnlyColumns()
        {
            return _Argument(Res.GetString(Res.DataTable_MultipleSimpleContentColumns));
        }

        public static Exception InvalidSortString(string sort)
        {
            return _Argument(Res.GetString(Res.DataTable_InvalidSortString, sort));
        }

        public static Exception DuplicateTableName(string table)
        {
            return _DuplicateName(Res.GetString(Res.DataTable_DuplicateName, table));
        }

        public static Exception DuplicateTableName2(string table, string ns)
        {
            return _DuplicateName(Res.GetString(Res.DataTable_DuplicateName2, table, ns));
        }

        public static Exception SelfnestedDatasetConflictingName(string table)
        {
            return _DuplicateName(
                Res.GetString(Res.DataTable_SelfnestedDatasetConflictingName, table)
            );
        }

        public static Exception DatasetConflictingName(string table)
        {
            return _DuplicateName(Res.GetString(Res.DataTable_DatasetConflictingName, table));
        }

        public static Exception TableAlreadyInOtherDataSet()
        {
            return _Argument(Res.GetString(Res.DataTable_AlreadyInOtherDataSet));
        }

        public static Exception TableAlreadyInTheDataSet()
        {
            return _Argument(Res.GetString(Res.DataTable_AlreadyInTheDataSet));
        }

        public static Exception TableOutOfRange(int index)
        {
            return _IndexOutOfRange(
                Res.GetString(
                    Res.DataTable_OutOfRange,
                    (index).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception TableNotInTheDataSet(string table)
        {
            return _Argument(Res.GetString(Res.DataTable_NotInTheDataSet, table));
        }

        public static Exception TableInRelation()
        {
            return _Argument(Res.GetString(Res.DataTable_InRelation));
        }

        public static Exception TableInConstraint(DataTable table, Constraint constraint)
        {
            return _Argument(
                Res.GetString(
                    Res.DataTable_InConstraint,
                    table.TableName,
                    constraint.ConstraintName
                )
            );
        }

        public static Exception CanNotSerializeDataTableHierarchy()
        {
            return _InvalidOperation(
                Res.GetString(Res.DataTable_CanNotSerializeDataTableHierarchy)
            );
        }

        public static Exception CanNotRemoteDataTable()
        {
            return _InvalidOperation(Res.GetString(Res.DataTable_CanNotRemoteDataTable));
        }

        public static Exception CanNotSetRemotingFormat()
        {
            return _Argument(Res.GetString(Res.DataTable_CanNotSetRemotingFormat));
        }

        public static Exception CanNotSerializeDataTableWithEmptyName()
        {
            return _InvalidOperation(
                Res.GetString(Res.DataTable_CanNotSerializeDataTableWithEmptyName)
            );
        }

        public static Exception TableNotFound(string tableName)
        {
            return _Argument(Res.GetString(Res.DataTable_TableNotFound, tableName));
        }

        //
        // Storage
        //
        static public Exception AggregateException(AggregateType aggregateType, Type type)
        {
            return _Data(
                Res.GetString(
                    Res.DataStorage_AggregateException,
                    aggregateType.ToString(),
                    type.Name
                )
            );
        }

        public static Exception InvalidStorageType(TypeCode typecode)
        {
            return _Data(
                Res.GetString(Res.DataStorage_InvalidStorageType, ((Enum)typecode).ToString())
            );
        }

        public static Exception RangeArgument(Int32 min, Int32 max)
        {
            return _Argument(
                Res.GetString(
                    Res.Range_Argument,
                    (min).ToString(CultureInfo.InvariantCulture),
                    (max).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception NullRange()
        {
            return _Data(Res.GetString(Res.Range_NullRange));
        }

        public static Exception NegativeMinimumCapacity()
        {
            return _Argument(Res.GetString(Res.RecordManager_MinimumCapacity));
        }

        public static Exception ProblematicChars(char charValue)
        {
            string xchar = "0x" + ((UInt16)charValue).ToString("X", CultureInfo.InvariantCulture);
            return _Argument(Res.GetString(Res.DataStorage_ProblematicChars, xchar));
        }

        public static Exception StorageSetFailed()
        {
            return _Argument(Res.GetString(Res.DataStorage_SetInvalidDataType));
        }

        //
        // XML schema
        //
        static public Exception SimpleTypeNotSupported()
        {
            return _Data(Res.GetString(Res.Xml_SimpleTypeNotSupported));
        }

        public static Exception MissingAttribute(string attribute)
        {
            return MissingAttribute(String.Empty, attribute);
        }

        public static Exception MissingAttribute(string element, string attribute)
        {
            return _Data(Res.GetString(Res.Xml_MissingAttribute, element, attribute));
        }

        public static Exception InvalidAttributeValue(string name, string value)
        {
            return _Data(Res.GetString(Res.Xml_ValueOutOfRange, name, value));
        }

        public static Exception AttributeValues(string name, string value1, string value2)
        {
            return _Data(Res.GetString(Res.Xml_AttributeValues, name, value1, value2));
        }

        public static Exception ElementTypeNotFound(string name)
        {
            return _Data(Res.GetString(Res.Xml_ElementTypeNotFound, name));
        }

        public static Exception RelationParentNameMissing(string rel)
        {
            return _Data(Res.GetString(Res.Xml_RelationParentNameMissing, rel));
        }

        public static Exception RelationChildNameMissing(string rel)
        {
            return _Data(Res.GetString(Res.Xml_RelationChildNameMissing, rel));
        }

        public static Exception RelationTableKeyMissing(string rel)
        {
            return _Data(Res.GetString(Res.Xml_RelationTableKeyMissing, rel));
        }

        public static Exception RelationChildKeyMissing(string rel)
        {
            return _Data(Res.GetString(Res.Xml_RelationChildKeyMissing, rel));
        }

        public static Exception UndefinedDatatype(string name)
        {
            return _Data(Res.GetString(Res.Xml_UndefinedDatatype, name));
        }

        public static Exception DatatypeNotDefined()
        {
            return _Data(Res.GetString(Res.Xml_DatatypeNotDefined));
        }

        public static Exception MismatchKeyLength()
        {
            return _Data(Res.GetString(Res.Xml_MismatchKeyLength));
        }

        public static Exception InvalidField(string name)
        {
            return _Data(Res.GetString(Res.Xml_InvalidField, name));
        }

        public static Exception InvalidSelector(string name)
        {
            return _Data(Res.GetString(Res.Xml_InvalidSelector, name));
        }

        public static Exception CircularComplexType(string name)
        {
            return _Data(Res.GetString(Res.Xml_CircularComplexType, name));
        }

        public static Exception CannotInstantiateAbstract(string name)
        {
            return _Data(Res.GetString(Res.Xml_CannotInstantiateAbstract, name));
        }

        public static Exception InvalidKey(string name)
        {
            return _Data(Res.GetString(Res.Xml_InvalidKey, name));
        }

        public static Exception DiffgramMissingTable(string name)
        {
            return _Data(Res.GetString(Res.Xml_MissingTable, name));
        }

        public static Exception DiffgramMissingSQL()
        {
            return _Data(Res.GetString(Res.Xml_MissingSQL));
        }

        public static Exception DuplicateConstraintRead(string str)
        {
            return _Data(Res.GetString(Res.Xml_DuplicateConstraint, str));
        }

        public static Exception ColumnTypeConflict(string name)
        {
            return _Data(Res.GetString(Res.Xml_ColumnConflict, name));
        }

        public static Exception CannotConvert(string name, string type)
        {
            return _Data(Res.GetString(Res.Xml_CannotConvert, name, type));
        }

        public static Exception MissingRefer(string name)
        {
            return _Data(
                Res.GetString(Res.Xml_MissingRefer, Keywords.REFER, Keywords.XSD_KEYREF, name)
            );
        }

        public static Exception InvalidPrefix(string name)
        {
            return _Data(Res.GetString(Res.Xml_InvalidPrefix, name));
        }

        public static Exception CanNotDeserializeObjectType()
        {
            return _InvalidOperation(Res.GetString(Res.Xml_CanNotDeserializeObjectType));
        }

        public static Exception IsDataSetAttributeMissingInSchema()
        {
            return _Data(Res.GetString(Res.Xml_IsDataSetAttributeMissingInSchema));
        }

        public static Exception TooManyIsDataSetAtributeInSchema()
        {
            return _Data(Res.GetString(Res.Xml_TooManyIsDataSetAtributeInSchema));
        }

        // XML save
        static public Exception NestedCircular(string name)
        {
            return _Data(Res.GetString(Res.Xml_NestedCircular, name));
        }

        public static Exception MultipleParentRows(string tableQName)
        {
            return _Data(Res.GetString(Res.Xml_MultipleParentRows, tableQName));
        }

        public static Exception PolymorphismNotSupported(string typeName)
        {
            return _InvalidOperation(Res.GetString(Res.Xml_PolymorphismNotSupported, typeName));
        }

        public static Exception DataTableInferenceNotSupported()
        {
            return _InvalidOperation(Res.GetString(Res.Xml_DataTableInferenceNotSupported));
        }

        /// <summary>throw DataException for multitarget failure</summary>
        /// <param name="innerException">exception from multitarget converter</param>
        /// <exception cref="DataException">always thrown</exception>
        static internal void ThrowMultipleTargetConverter(Exception innerException)
        {
            string res =
                (null != innerException)
                    ? Res.Xml_MultipleTargetConverterError
                    : Res.Xml_MultipleTargetConverterEmpty;
            ThrowDataException(Res.GetString(res), innerException);
        }

        //
        // Merge
        //
        static public Exception DuplicateDeclaration(string name)
        {
            return _Data(Res.GetString(Res.Xml_MergeDuplicateDeclaration, name));
        }

        //Read Xml data
        static public Exception FoundEntity()
        {
            return _Data(Res.GetString(Res.Xml_FoundEntity));
        }

        // ATTENTION: name has to be localized string here:
        static public Exception MergeFailed(string name)
        {
            return _Data(name);
        }

        // SqlConvert
        static public DataException ConvertFailed(Type type1, Type type2)
        {
            return _Data(
                Res.GetString(Res.SqlConvert_ConvertFailed, type1.FullName, type2.FullName)
            );
        }

        // DataTableReader
        static public Exception InvalidDataTableReader(string tableName)
        {
            return _InvalidOperation(
                Res.GetString(Res.DataTableReader_InvalidDataTableReader, tableName)
            );
        }

        public static Exception DataTableReaderSchemaIsInvalid(string tableName)
        {
            return _InvalidOperation(
                Res.GetString(Res.DataTableReader_SchemaInvalidDataTableReader, tableName)
            );
        }

        public static Exception CannotCreateDataReaderOnEmptyDataSet()
        {
            return _Argument(
                Res.GetString(Res.DataTableReader_CannotCreateDataReaderOnEmptyDataSet)
            );
        }

        public static Exception DataTableReaderArgumentIsEmpty()
        {
            return _Argument(Res.GetString(Res.DataTableReader_DataTableReaderArgumentIsEmpty));
        }

        public static Exception ArgumentContainsNullValue()
        {
            return _Argument(Res.GetString(Res.DataTableReader_ArgumentContainsNullValue));
        }

        public static Exception InvalidCurrentRowInDataTableReader()
        {
            return _DeletedRowInaccessible(
                Res.GetString(Res.DataTableReader_InvalidRowInDataTableReader)
            );
        }

        public static Exception EmptyDataTableReader(string tableName)
        {
            return _DeletedRowInaccessible(
                Res.GetString(Res.DataTableReader_DataTableCleared, tableName)
            );
        }

        //
        static internal Exception InvalidDuplicateNamedSimpleTypeDelaration(
            string stName,
            string errorStr
        )
        {
            return _Argument(
                Res.GetString(
                    Res.NamedSimpleType_InvalidDuplicateNamedSimpleTypeDelaration,
                    stName,
                    errorStr
                )
            );
        }

        // RbTree
        static internal Exception InternalRBTreeError(RBTreeError internalError)
        {
            return _InvalidOperation(Res.GetString(Res.RbTree_InvalidState, (int)internalError));
        }

        public static Exception EnumeratorModified()
        {
            return _InvalidOperation(Res.GetString(Res.RbTree_EnumerationBroken));
        }
    } // ExceptionBuilder
}
