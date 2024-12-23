//------------------------------------------------------------------------------
// <copyright file="SqlUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ExceptionServices;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Versioning;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using SysTx = System.Transactions;

    internal static class AsyncHelper
    {
#if !MONO
        internal static Task CreateContinuationTask(
            Task task,
            Action onSuccess,
            SqlInternalConnectionTds connectionToDoom = null,
            Action<Exception> onFailure = null
        )
        {
            if (task == null)
            {
                onSuccess();
                return null;
            }
            else
            {
                TaskCompletionSource<object> completion = new TaskCompletionSource<object>();
                ContinueTask(
                    task,
                    completion,
                    () =>
                    {
                        onSuccess();
                        completion.SetResult(null);
                    },
                    connectionToDoom,
                    onFailure
                );
                return completion.Task;
            }
        }

        internal static Task CreateContinuationTask<T1, T2>(
            Task task,
            Action<T1, T2> onSuccess,
            T1 arg1,
            T2 arg2,
            SqlInternalConnectionTds connectionToDoom = null,
            Action<Exception> onFailure = null
        )
        {
            return CreateContinuationTask(
                task,
                () => onSuccess(arg1, arg2),
                connectionToDoom,
                onFailure
            );
        }

        internal static void ContinueTask(
            Task task,
            TaskCompletionSource<object> completion,
            Action onSuccess,
            SqlInternalConnectionTds connectionToDoom = null,
            Action<Exception> onFailure = null,
            Action onCancellation = null,
            Func<Exception, Exception> exceptionConverter = null,
            SqlConnection connectionToAbort = null
        )
        {
            Debug.Assert(
                (connectionToAbort == null) || (connectionToDoom == null),
                "Should not specify both connectionToDoom and connectionToAbort"
            );
            task.ContinueWith(
                tsk =>
                {
                    if (tsk.Exception != null)
                    {
                        Exception exc = tsk.Exception.InnerException;
                        if (exceptionConverter != null)
                        {
                            exc = exceptionConverter(exc);
                        }
                        try
                        {
                            if (onFailure != null)
                            {
                                onFailure(exc);
                            }
                        }
                        finally
                        {
                            completion.TrySetException(exc);
                        }
                    }
                    else if (tsk.IsCanceled)
                    {
                        try
                        {
                            if (onCancellation != null)
                            {
                                onCancellation();
                            }
                        }
                        finally
                        {
                            completion.TrySetCanceled();
                        }
                    }
                    else
                    {
                        if (connectionToDoom != null || connectionToAbort != null)
                        {
                            RuntimeHelpers.PrepareConstrainedRegions();
                            try
                            {
#if DEBUG
                                TdsParser.ReliabilitySection tdsReliabilitySection =
                                    new TdsParser.ReliabilitySection();
                                RuntimeHelpers.PrepareConstrainedRegions();
                                try
                                {
                                    tdsReliabilitySection.Start();
#endif //DEBUG
                                    onSuccess();
#if DEBUG
                                }
                                finally
                                {
                                    tdsReliabilitySection.Stop();
                                }
#endif //DEBUG
                            }
                            catch (System.OutOfMemoryException e)
                            {
                                if (connectionToDoom != null)
                                {
                                    connectionToDoom.DoomThisConnection();
                                }
                                else
                                {
                                    connectionToAbort.Abort(e);
                                }
                                completion.SetException(e);
                                throw;
                            }
                            catch (System.StackOverflowException e)
                            {
                                if (connectionToDoom != null)
                                {
                                    connectionToDoom.DoomThisConnection();
                                }
                                else
                                {
                                    connectionToAbort.Abort(e);
                                }
                                completion.SetException(e);
                                throw;
                            }
                            catch (System.Threading.ThreadAbortException e)
                            {
                                if (connectionToDoom != null)
                                {
                                    connectionToDoom.DoomThisConnection();
                                }
                                else
                                {
                                    connectionToAbort.Abort(e);
                                }
                                completion.SetException(e);
                                throw;
                            }
                            catch (Exception e)
                            {
                                completion.SetException(e);
                            }
                        }
                        else
                        { // no connection to doom - reliability section not required
                            try
                            {
                                onSuccess();
                            }
                            catch (Exception e)
                            {
                                completion.SetException(e);
                            }
                        }
                    }
                },
                TaskScheduler.Default
            );
        }
#endif

        internal static void WaitForCompletion(
            Task task,
            int timeout,
            Action onTimeout = null,
            bool rethrowExceptions = true
        )
        {
            try
            {
                task.Wait(timeout > 0 ? (1000 * timeout) : Timeout.Infinite);
            }
            catch (AggregateException ae)
            {
                if (rethrowExceptions)
                {
                    Debug.Assert(
                        ae.InnerExceptions.Count == 1,
                        "There is more than one exception in AggregateException"
                    );
                    ExceptionDispatchInfo.Capture(ae.InnerException).Throw();
                }
            }
            if (!task.IsCompleted)
            {
                if (onTimeout != null)
                {
                    onTimeout();
                }
            }
        }

        internal static void SetTimeoutException(
            TaskCompletionSource<object> completion,
            int timeout,
            Func<Exception> exc,
            CancellationToken ctoken
        )
        {
            if (timeout > 0)
            {
                Task.Delay(timeout * 1000, ctoken)
                    .ContinueWith(
                        (tsk) =>
                        {
                            if (!tsk.IsCanceled && !completion.Task.IsCompleted)
                            {
                                completion.TrySetException(exc());
                            }
                        }
                    );
            }
        }
    }

    internal sealed class InOutOfProcHelper
    {
        private static readonly InOutOfProcHelper SingletonInstance = new InOutOfProcHelper();

        private bool _inProc = false;

        // InOutOfProcHelper detects whether it's running inside the server or not.  It does this
        //  by checking for the existence of a well-known function export on the current process.
        //  Note that calling conventions, etc. do not matter -- we'll never call the function, so
        //  only the name match or lack thereof matter.
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        private InOutOfProcHelper()
        {
#if !MONO
            // Don't need to close this handle...
            // SxS: we use this method to check if we are running inside the SQL Server process. This call should be safe in SxS environment.
            IntPtr handle = SafeNativeMethods.GetModuleHandle(null);
            if (IntPtr.Zero != handle)
            {
                // SQLBU 359301: Currently, the server exports different names for x86 vs. AMD64 and IA64.  Supporting both names
                //  for now gives the server time to unify names across platforms without breaking currently-working ones.
                //  We can remove the obsolete name once the server is changed.
                if (
                    IntPtr.Zero
                    != SafeNativeMethods.GetProcAddress(
                        handle,
                        "_______SQL______Process______Available@0"
                    )
                )
                {
                    _inProc = true;
                }
                else if (
                    IntPtr.Zero
                    != SafeNativeMethods.GetProcAddress(
                        handle,
                        "______SQL______Process______Available"
                    )
                )
                {
                    _inProc = true;
                }
            }
#endif
        }

        internal static bool InProc
        {
            get { return SingletonInstance._inProc; }
        }
    }

    internal static class SQL
    {
        // The class SQL defines the exceptions that are specific to the SQL Adapter.
        // The class contains functions that take the proper informational variables and then construct
        // the appropriate exception with an error string obtained from the resource Framework.txt.
        // The exception is then returned to the caller, so that the caller may then throw from its
        // location so that the catcher of the exception will have the appropriate call stack.
        // This class is used so that there will be compile time checking of error
        // messages.  The resource Framework.txt will ensure proper string text based on the appropriate
        // locale.

        //
        // SQL specific exceptions
        //

        //
        // SQL.Connection
        //

        static internal Exception CannotGetDTCAddress()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_CannotGetDTCAddress));
        }

        internal static Exception InvalidOptionLength(string key)
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidOptionLength, key));
        }

        internal static Exception InvalidInternalPacketSize(string str)
        {
            return ADP.ArgumentOutOfRange(str);
        }

        internal static Exception InvalidPacketSize()
        {
            return ADP.ArgumentOutOfRange(Res.GetString(Res.SQL_InvalidTDSPacketSize));
        }

        internal static Exception InvalidPacketSizeValue()
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidPacketSizeValue));
        }

        internal static Exception InvalidSSPIPacketSize()
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidSSPIPacketSize));
        }

        internal static Exception NullEmptyTransactionName()
        {
            return ADP.Argument(Res.GetString(Res.SQL_NullEmptyTransactionName));
        }

        internal static Exception SnapshotNotSupported(IsolationLevel level)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_SnapshotNotSupported,
                    typeof(IsolationLevel),
                    level.ToString()
                )
            );
        }

        internal static Exception UserInstanceFailoverNotCompatible()
        {
            return ADP.Argument(Res.GetString(Res.SQL_UserInstanceFailoverNotCompatible));
        }

        internal static Exception CredentialsNotProvided(SqlAuthenticationMethod auth)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_CredentialsNotProvided,
                    DbConnectionStringBuilderUtil.AuthenticationTypeToString(auth)
                )
            );
        }

        internal static Exception AuthenticationAndIntegratedSecurity()
        {
            return ADP.Argument(Res.GetString(Res.SQL_AuthenticationAndIntegratedSecurity));
        }

        internal static Exception IntegratedWithUserIDAndPassword()
        {
            return ADP.Argument(Res.GetString(Res.SQL_IntegratedWithUserIDAndPassword));
        }

        internal static Exception SettingIntegratedWithCredential()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SettingIntegratedWithCredential));
        }

        internal static Exception SettingCredentialWithIntegratedArgument()
        {
            return ADP.Argument(Res.GetString(Res.SQL_SettingCredentialWithIntegrated));
        }

        internal static Exception SettingCredentialWithIntegratedInvalid()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SettingCredentialWithIntegrated));
        }

        internal static Exception InvalidSQLServerVersionUnknown()
        {
            return ADP.DataAdapter(Res.GetString(Res.SQL_InvalidSQLServerVersionUnknown));
        }

        internal static Exception SynchronousCallMayNotPend()
        {
            return new Exception(Res.GetString(Res.Sql_InternalError));
        }

        internal static Exception ConnectionLockedForBcpEvent()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ConnectionLockedForBcpEvent));
        }

        internal static Exception AsyncConnectionRequired()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_AsyncConnectionRequired));
        }

        internal static Exception FatalTimeout()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_FatalTimeout));
        }

        internal static Exception InstanceFailure()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_InstanceFailure));
        }

        internal static Exception ChangePasswordArgumentMissing(string argumentName)
        {
            return ADP.ArgumentNull(
                Res.GetString(Res.SQL_ChangePasswordArgumentMissing, argumentName)
            );
        }

        internal static Exception ChangePasswordConflictsWithSSPI()
        {
            return ADP.Argument(Res.GetString(Res.SQL_ChangePasswordConflictsWithSSPI));
        }

        internal static Exception ChangePasswordRequiresYukon()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ChangePasswordRequiresYukon));
        }

        internal static Exception UnknownSysTxIsolationLevel(SysTx.IsolationLevel isolationLevel)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_UnknownSysTxIsolationLevel, isolationLevel.ToString())
            );
        }

        internal static Exception ChangePasswordUseOfUnallowedKey(string key)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_ChangePasswordUseOfUnallowedKey, key)
            );
        }

        internal static Exception InvalidPartnerConfiguration(string server, string database)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_InvalidPartnerConfiguration, server, database)
            );
        }

        internal static Exception BatchedUpdateColumnEncryptionSettingMismatch()
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.TCE_BatchedUpdateColumnEncryptionSettingMismatch,
                    "SqlCommandColumnEncryptionSetting",
                    "SelectCommand",
                    "InsertCommand",
                    "UpdateCommand",
                    "DeleteCommand"
                )
            );
        }

        internal static Exception MARSUnspportedOnConnection()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_MarsUnsupportedOnConnection));
        }

        internal static Exception CannotModifyPropertyAsyncOperationInProgress(string property)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_CannotModifyPropertyAsyncOperationInProgress, property)
            );
        }

        internal static Exception NonLocalSSEInstance()
        {
            return ADP.NotSupported(Res.GetString(Res.SQL_NonLocalSSEInstance));
        }

        //
        // SQL.DataCommand
        //
        static internal Exception NotificationsRequireYukon()
        {
            return ADP.NotSupported(Res.GetString(Res.SQL_NotificationsRequireYukon));
        }

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(
            Type type,
            int value
        )
        {
            return ADP.ArgumentOutOfRange(
                Res.GetString(
                    Res.SQL_NotSupportedEnumerationValue,
                    type.Name,
                    value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                ),
                type.Name
            );
        }

        internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value)
        {
#if DEBUG
            switch (value)
            {
                case CommandType.Text:
                case CommandType.StoredProcedure:
                    Debug.Assert(false, "valid CommandType " + value.ToString());
                    break;
                case CommandType.TableDirect:
                    break;
                default:
                    Debug.Assert(false, "invalid CommandType " + value.ToString());
                    break;
            }
#endif
            return NotSupportedEnumerationValue(typeof(CommandType), (int)value);
        }

        internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(IsolationLevel value)
        {
#if DEBUG
            switch (value)
            {
                case IsolationLevel.Unspecified:
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.ReadUncommitted:
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Serializable:
                case IsolationLevel.Snapshot:
                    Debug.Assert(false, "valid IsolationLevel " + value.ToString());
                    break;
                case IsolationLevel.Chaos:
                    break;
                default:
                    Debug.Assert(false, "invalid IsolationLevel " + value.ToString());
                    break;
            }
#endif
            return NotSupportedEnumerationValue(typeof(IsolationLevel), (int)value);
        }

        internal static Exception OperationCancelled()
        {
            Exception exception = ADP.InvalidOperation(Res.GetString(Res.SQL_OperationCancelled));
            return exception;
        }

        internal static Exception PendingBeginXXXExists()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_PendingBeginXXXExists));
        }

        internal static ArgumentOutOfRangeException InvalidSqlDependencyTimeout(string param)
        {
            return ADP.ArgumentOutOfRange(Res.GetString(Res.SqlDependency_InvalidTimeout), param);
        }

        internal static Exception NonXmlResult()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_NonXmlResult));
        }

        //
        // SQL.DataParameter
        //
        static internal Exception InvalidUdt3PartNameFormat()
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidUdt3PartNameFormat));
        }

        internal static Exception InvalidParameterTypeNameFormat()
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidParameterTypeNameFormat));
        }

        internal static Exception InvalidParameterNameLength(string value)
        {
            return ADP.Argument(Res.GetString(Res.SQL_InvalidParameterNameLength, value));
        }

        internal static Exception PrecisionValueOutOfRange(byte precision)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_PrecisionValueOutOfRange,
                    precision.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        internal static Exception ScaleValueOutOfRange(byte scale)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_ScaleValueOutOfRange,
                    scale.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        internal static Exception TimeScaleValueOutOfRange(byte scale)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_TimeScaleValueOutOfRange,
                    scale.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        internal static Exception InvalidSqlDbType(SqlDbType value)
        {
            return ADP.InvalidEnumerationValue(typeof(SqlDbType), (int)value);
        }

        internal static Exception UnsupportedTVPOutputParameter(
            ParameterDirection direction,
            string paramName
        )
        {
            return ADP.NotSupported(
                Res.GetString(
                    Res.SqlParameter_UnsupportedTVPOutputParameter,
                    direction.ToString(),
                    paramName
                )
            );
        }

        internal static Exception DBNullNotSupportedForTVPValues(string paramName)
        {
            return ADP.NotSupported(
                Res.GetString(Res.SqlParameter_DBNullNotSupportedForTVP, paramName)
            );
        }

        internal static Exception InvalidTableDerivedPrecisionForTvp(
            string columnName,
            byte precision
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SqlParameter_InvalidTableDerivedPrecisionForTvp,
                    precision,
                    columnName,
                    System.Data.SqlTypes.SqlDecimal.MaxPrecision
                )
            );
        }

        internal static Exception UnexpectedTypeNameForNonStructParams(string paramName)
        {
            return ADP.NotSupported(
                Res.GetString(Res.SqlParameter_UnexpectedTypeNameForNonStruct, paramName)
            );
        }

        internal static Exception SingleValuedStructNotSupported()
        {
            return ADP.NotSupported(Res.GetString(Res.MetaType_SingleValuedStructNotSupported));
        }

        internal static Exception ParameterInvalidVariant(string paramName)
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ParameterInvalidVariant, paramName));
        }

        internal static Exception MustSetTypeNameForParam(string paramType, string paramName)
        {
            return ADP.Argument(
                Res.GetString(Res.SQL_ParameterTypeNameRequired, paramType, paramName)
            );
        }

        internal static Exception NullSchemaTableDataTypeNotSupported(string columnName)
        {
            return ADP.Argument(Res.GetString(Res.NullSchemaTableDataTypeNotSupported, columnName));
        }

        internal static Exception InvalidSchemaTableOrdinals()
        {
            return ADP.Argument(Res.GetString(Res.InvalidSchemaTableOrdinals));
        }

        internal static Exception EnumeratedRecordMetaDataChanged(
            string fieldName,
            int recordNumber
        )
        {
            return ADP.Argument(
                Res.GetString(Res.SQL_EnumeratedRecordMetaDataChanged, fieldName, recordNumber)
            );
        }

        internal static Exception EnumeratedRecordFieldCountChanged(int recordNumber)
        {
            return ADP.Argument(
                Res.GetString(Res.SQL_EnumeratedRecordFieldCountChanged, recordNumber)
            );
        }

        //
        // SQL.SqlDataAdapter
        //

        //
        // SQL.TDSParser
        //
        static internal Exception InvalidTDSVersion()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_InvalidTDSVersion));
        }

        internal static Exception ParsingError(ParsingErrorState state)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorWithState,
                    ((int)state).ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        internal static Exception ParsingError(ParsingErrorState state, Exception innerException)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorWithState,
                    ((int)state).ToString(CultureInfo.InvariantCulture)
                ),
                innerException
            );
        }

        internal static Exception ParsingErrorValue(ParsingErrorState state, int value)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorValue,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    value
                )
            );
        }

        internal static Exception ParsingErrorOffset(ParsingErrorState state, int offset)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorOffset,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    offset
                )
            );
        }

        internal static Exception ParsingErrorFeatureId(ParsingErrorState state, int featureId)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorFeatureId,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    featureId
                )
            );
        }

        internal static Exception ParsingErrorToken(ParsingErrorState state, int token)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorToken,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    token
                )
            );
        }

        internal static Exception ParsingErrorLength(ParsingErrorState state, int length)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorLength,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    length
                )
            );
        }

        internal static Exception ParsingErrorStatus(ParsingErrorState state, int status)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorStatus,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    status
                )
            );
        }

        internal static Exception ParsingErrorLibraryType(ParsingErrorState state, int libraryType)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_ParsingErrorAuthLibraryType,
                    ((int)state).ToString(CultureInfo.InvariantCulture),
                    libraryType
                )
            );
        }

        internal static Exception MoneyOverflow(string moneyValue)
        {
            return ADP.Overflow(Res.GetString(Res.SQL_MoneyOverflow, moneyValue));
        }

        internal static Exception SmallDateTimeOverflow(string datetime)
        {
            return ADP.Overflow(Res.GetString(Res.SQL_SmallDateTimeOverflow, datetime));
        }

        internal static Exception SNIPacketAllocationFailure()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SNIPacketAllocationFailure));
        }

        internal static Exception TimeOverflow(string time)
        {
            return ADP.Overflow(Res.GetString(Res.SQL_TimeOverflow, time));
        }

        //
        // SQL.SqlDataReader
        //
        static internal Exception InvalidRead()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_InvalidRead));
        }

        internal static Exception NonBlobColumn(string columnName)
        {
            return ADP.InvalidCast(Res.GetString(Res.SQL_NonBlobColumn, columnName));
        }

        internal static Exception NonCharColumn(string columnName)
        {
            return ADP.InvalidCast(Res.GetString(Res.SQL_NonCharColumn, columnName));
        }

        internal static Exception StreamNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(Res.GetString(Res.SQL_StreamNotSupportOnColumnType, columnName));
        }

        internal static Exception StreamNotSupportOnEncryptedColumn(string columnName)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.TCE_StreamNotSupportOnEncryptedColumn, columnName, "Stream")
            );
        }

        internal static Exception SequentialAccessNotSupportedOnEncryptedColumn(string columnName)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.TCE_SequentialAccessNotSupportedOnEncryptedColumn,
                    columnName,
                    "CommandBehavior=SequentialAccess"
                )
            );
        }

        internal static Exception TextReaderNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(
                Res.GetString(Res.SQL_TextReaderNotSupportOnColumnType, columnName)
            );
        }

        internal static Exception XmlReaderNotSupportOnColumnType(string columnName)
        {
            return ADP.InvalidCast(
                Res.GetString(Res.SQL_XmlReaderNotSupportOnColumnType, columnName)
            );
        }

        internal static Exception UDTUnexpectedResult(string exceptionText)
        {
            return ADP.TypeLoad(Res.GetString(Res.SQLUDT_Unexpected, exceptionText));
        }

        //
        // SQL.SqlDelegatedTransaction
        //
        static internal Exception CannotCompleteDelegatedTransactionWithOpenResults(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    TdsEnums.TIMEOUT_EXPIRED,
                    (byte)0x00,
                    TdsEnums.MIN_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.ADP_OpenReaderExists)),
                    "",
                    0,
                    TdsEnums.SNI_WAIT_TIMEOUT
                )
            );
            return SqlException.CreateException(errors, null, internalConnection);
        }

        internal static SysTx.TransactionPromotionException PromotionFailed(Exception inner)
        {
            SysTx.TransactionPromotionException e = new SysTx.TransactionPromotionException(
                Res.GetString(Res.SqlDelegatedTransaction_PromotionFailed),
                inner
            );
            ADP.TraceExceptionAsReturnValue(e);
            return e;
        }

        //
        // SQL.SqlDependency
        //
        static internal Exception SqlCommandHasExistingSqlNotificationRequest()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQLNotify_AlreadyHasCommand));
        }

        internal static Exception SqlDepCannotBeCreatedInProc()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlNotify_SqlDepCannotBeCreatedInProc));
        }

        internal static Exception SqlDepDefaultOptionsButNoStart()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_DefaultOptionsButNoStart));
        }

        internal static Exception SqlDependencyDatabaseBrokerDisabled()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_DatabaseBrokerDisabled));
        }

        internal static Exception SqlDependencyEventNoDuplicate()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_EventNoDuplicate));
        }

        internal static Exception SqlDependencyDuplicateStart()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_DuplicateStart));
        }

        internal static Exception SqlDependencyIdMismatch()
        {
            // do not include the id because it may require SecurityPermission(Infrastructure) permission
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_IdMismatch));
        }

        internal static Exception SqlDependencyNoMatchingServerStart()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlDependency_NoMatchingServerStart));
        }

        internal static Exception SqlDependencyNoMatchingServerDatabaseStart()
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlDependency_NoMatchingServerDatabaseStart)
            );
        }

        internal static Exception SqlNotificationException(SqlNotificationEventArgs notify)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQLNotify_ErrorFormat, notify.Type, notify.Info, notify.Source)
            );
        }

        //
        // SQL.SqlMetaData
        //
        static internal Exception SqlMetaDataNoMetaData()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlMetaData_NoMetadata));
        }

        internal static Exception MustSetUdtTypeNameForUdtParams()
        {
            return ADP.Argument(Res.GetString(Res.SQLUDT_InvalidUdtTypeName));
        }

        internal static Exception UnexpectedUdtTypeNameForNonUdtParams()
        {
            return ADP.Argument(Res.GetString(Res.SQLUDT_UnexpectedUdtTypeName));
        }

        internal static Exception UDTInvalidSqlType(string typeName)
        {
            return ADP.Argument(Res.GetString(Res.SQLUDT_InvalidSqlType, typeName));
        }

        internal static Exception InvalidSqlDbTypeForConstructor(SqlDbType type)
        {
            return ADP.Argument(
                Res.GetString(Res.SqlMetaData_InvalidSqlDbTypeForConstructorFormat, type.ToString())
            );
        }

        internal static Exception NameTooLong(string parameterName)
        {
            return ADP.Argument(Res.GetString(Res.SqlMetaData_NameTooLong), parameterName);
        }

        internal static Exception InvalidSortOrder(SortOrder order)
        {
            return ADP.InvalidEnumerationValue(typeof(SortOrder), (int)order);
        }

        internal static Exception MustSpecifyBothSortOrderAndOrdinal(SortOrder order, int ordinal)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SqlMetaData_SpecifyBothSortOrderAndOrdinal,
                    order.ToString(),
                    ordinal
                )
            );
        }

        internal static Exception TableTypeCanOnlyBeParameter()
        {
            return ADP.Argument(Res.GetString(Res.SQLTVP_TableTypeCanOnlyBeParameter));
        }

        internal static Exception UnsupportedColumnTypeForSqlProvider(
            string columnName,
            string typeName
        )
        {
            return ADP.Argument(
                Res.GetString(Res.SqlProvider_InvalidDataColumnType, columnName, typeName)
            );
        }

        internal static Exception InvalidColumnMaxLength(string columnName, long maxLength)
        {
            return ADP.Argument(
                Res.GetString(Res.SqlProvider_InvalidDataColumnMaxLength, columnName, maxLength)
            );
        }

        internal static Exception InvalidColumnPrecScale()
        {
            return ADP.Argument(Res.GetString(Res.SqlMisc_InvalidPrecScaleMessage));
        }

        internal static Exception NotEnoughColumnsInStructuredType()
        {
            return ADP.Argument(Res.GetString(Res.SqlProvider_NotEnoughColumnsInStructuredType));
        }

        internal static Exception DuplicateSortOrdinal(int sortOrdinal)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlProvider_DuplicateSortOrdinal, sortOrdinal)
            );
        }

        internal static Exception MissingSortOrdinal(int sortOrdinal)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlProvider_MissingSortOrdinal, sortOrdinal)
            );
        }

        internal static Exception SortOrdinalGreaterThanFieldCount(
            int columnOrdinal,
            int sortOrdinal
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SqlProvider_SortOrdinalGreaterThanFieldCount,
                    sortOrdinal,
                    columnOrdinal
                )
            );
        }

        internal static Exception IEnumerableOfSqlDataRecordHasNoRows()
        {
            return ADP.Argument(Res.GetString(Res.IEnumerableOfSqlDataRecordHasNoRows));
        }

        //
        //  SqlPipe
        //
        static internal Exception SqlPipeCommandHookedUpToNonContextConnection()
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlPipe_CommandHookedUpToNonContextConnection)
            );
        }

        internal static Exception SqlPipeMessageTooLong(int messageLength)
        {
            return ADP.Argument(Res.GetString(Res.SqlPipe_MessageTooLong, messageLength));
        }

        internal static Exception SqlPipeIsBusy()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlPipe_IsBusy));
        }

        internal static Exception SqlPipeAlreadyHasAnOpenResultSet(string methodName)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlPipe_AlreadyHasAnOpenResultSet, methodName)
            );
        }

        internal static Exception SqlPipeDoesNotHaveAnOpenResultSet(string methodName)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SqlPipe_DoesNotHaveAnOpenResultSet, methodName)
            );
        }

        //
        // : ISqlResultSet
        //
        static internal Exception SqlResultSetClosed(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlResultSetClosed2));
            }
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlResultSetClosed, methodname));
        }

        internal static Exception SqlResultSetNoData(string methodname)
        {
            return ADP.InvalidOperation(Res.GetString(Res.ADP_DataReaderNoData, methodname));
        }

        internal static Exception SqlRecordReadOnly(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlRecordReadOnly2));
            }
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlRecordReadOnly, methodname));
        }

        internal static Exception SqlResultSetRowDeleted(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlResultSetRowDeleted2));
            }
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlResultSetRowDeleted, methodname));
        }

        internal static Exception SqlResultSetCommandNotInSameConnection()
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_SqlResultSetCommandNotInSameConnection)
            );
        }

        internal static Exception SqlResultSetNoAcceptableCursor()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_SqlResultSetNoAcceptableCursor));
        }

        //
        // SQL.BulkLoad
        //
        static internal Exception BulkLoadMappingInaccessible()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadMappingInaccessible));
        }

        internal static Exception BulkLoadMappingsNamesOrOrdinalsOnly()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadMappingsNamesOrOrdinalsOnly));
        }

#if !MONO
        static internal Exception BulkLoadCannotConvertValue(
            Type sourcetype,
            MetaType metatype,
            Exception e
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.SQL_BulkLoadCannotConvertValue,
                    sourcetype.Name,
                    metatype.TypeName
                ),
                e
            );
        }
#endif

        static internal Exception BulkLoadNonMatchingColumnMapping()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadNonMatchingColumnMapping));
        }

        internal static Exception BulkLoadNonMatchingColumnName(string columnName)
        {
            return BulkLoadNonMatchingColumnName(columnName, null);
        }

        internal static Exception BulkLoadNonMatchingColumnName(string columnName, Exception e)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_BulkLoadNonMatchingColumnName, columnName),
                e
            );
        }

        internal static Exception BulkLoadStringTooLong()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadStringTooLong));
        }

        internal static Exception BulkLoadInvalidVariantValue()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadInvalidVariantValue));
        }

        internal static Exception BulkLoadInvalidTimeout(int timeout)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_BulkLoadInvalidTimeout,
                    timeout.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        internal static Exception BulkLoadExistingTransaction()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadExistingTransaction));
        }

        internal static Exception BulkLoadNoCollation()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadNoCollation));
        }

        internal static Exception BulkLoadConflictingTransactionOption()
        {
            return ADP.Argument(Res.GetString(Res.SQL_BulkLoadConflictingTransactionOption));
        }

        internal static Exception BulkLoadLcidMismatch(
            int sourceLcid,
            string sourceColumnName,
            int destinationLcid,
            string destinationColumnName
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.Sql_BulkLoadLcidMismatch,
                    sourceLcid,
                    sourceColumnName,
                    destinationLcid,
                    destinationColumnName
                )
            );
        }

        internal static Exception InvalidOperationInsideEvent()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadInvalidOperationInsideEvent));
        }

        internal static Exception BulkLoadMissingDestinationTable()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadMissingDestinationTable));
        }

        internal static Exception BulkLoadInvalidDestinationTable(string tableName, Exception inner)
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_BulkLoadInvalidDestinationTable, tableName),
                inner
            );
        }

        internal static Exception BulkLoadBulkLoadNotAllowDBNull(string columnName)
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadNotAllowDBNull, columnName));
        }

        internal static Exception BulkLoadPendingOperation()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_BulkLoadPendingOperation));
        }

        //
        // TCE - Certificate Store Provider Errors.
        //
        static internal Exception InvalidKeyEncryptionAlgorithm(
            string encryptionAlgorithm,
            string validEncryptionAlgorithm,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidKeyEncryptionAlgorithmSysErr,
                        encryptionAlgorithm,
                        validEncryptionAlgorithm
                    ),
                    TdsEnums.TCE_PARAM_ENCRYPTION_ALGORITHM
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidKeyEncryptionAlgorithm,
                        encryptionAlgorithm,
                        validEncryptionAlgorithm
                    ),
                    TdsEnums.TCE_PARAM_ENCRYPTION_ALGORITHM
                );
            }
        }

        internal static Exception NullKeyEncryptionAlgorithm(bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_ENCRYPTION_ALGORITHM,
                    Res.GetString(Res.TCE_NullKeyEncryptionAlgorithmSysErr)
                );
            }
            else
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_ENCRYPTION_ALGORITHM,
                    Res.GetString(Res.TCE_NullKeyEncryptionAlgorithm)
                );
            }
        }

        internal static Exception EmptyColumnEncryptionKey()
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_EmptyColumnEncryptionKey),
                TdsEnums.TCE_PARAM_COLUMNENCRYPTION_KEY
            );
        }

        internal static Exception NullColumnEncryptionKey()
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_COLUMNENCRYPTION_KEY,
                Res.GetString(Res.TCE_NullColumnEncryptionKey)
            );
        }

        internal static Exception EmptyEncryptedColumnEncryptionKey()
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_EmptyEncryptedColumnEncryptionKey),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception NullEncryptedColumnEncryptionKey()
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK,
                Res.GetString(Res.TCE_NullEncryptedColumnEncryptionKey)
            );
        }

        internal static Exception LargeCertificatePathLength(
            int actualLength,
            int maxLength,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_LargeCertificatePathLengthSysErr,
                        actualLength,
                        maxLength
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_LargeCertificatePathLength, actualLength, maxLength),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception NullCertificatePath(string[] validLocations, bool isSystemOp)
        {
            Debug.Assert(2 == validLocations.Length);
            if (isSystemOp)
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(
                        Res.TCE_NullCertificatePathSysErr,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    )
                );
            }
            else
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(
                        Res.TCE_NullCertificatePath,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    )
                );
            }
        }

        internal static Exception NullCspKeyPath(bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(Res.TCE_NullCspPathSysErr, @"/")
                );
            }
            else
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(Res.TCE_NullCspPath, @"/")
                );
            }
        }

        internal static Exception NullCngKeyPath(bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(Res.TCE_NullCngPathSysErr, @"/")
                );
            }
            else
            {
                return ADP.ArgumentNull(
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH,
                    Res.GetString(Res.TCE_NullCngPath, @"/")
                );
            }
        }

        internal static Exception InvalidCertificatePath(
            string actualCertificatePath,
            string[] validLocations,
            bool isSystemOp
        )
        {
            Debug.Assert(2 == validLocations.Length);
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificatePathSysErr,
                        actualCertificatePath,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificatePath,
                        actualCertificatePath,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCspPath(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspPathSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspPath, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCngPath(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCngPathSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCngPath, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception EmptyCspName(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCspNameSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCspName, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception EmptyCngName(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCngNameSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCngName, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception EmptyCspKeyId(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCspKeyIdSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCspKeyId, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception EmptyCngKeyId(string masterKeyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCngKeyIdSysErr, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCngKeyId, masterKeyPath, @"/"),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCspName(
            string cspName,
            string masterKeyPath,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspNameSysErr, cspName, masterKeyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspName, cspName, masterKeyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCspKeyIdentifier(
            string keyIdentifier,
            string masterKeyPath,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspKeyIdSysErr, keyIdentifier, masterKeyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_InvalidCspKeyId, keyIdentifier, masterKeyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCngKey(
            string masterKeyPath,
            string cngProviderName,
            string keyIdentifier,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCngKeySysErr,
                        masterKeyPath,
                        cngProviderName,
                        keyIdentifier
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCngKey,
                        masterKeyPath,
                        cngProviderName,
                        keyIdentifier
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCertificateLocation(
            string certificateLocation,
            string certificatePath,
            string[] validLocations,
            bool isSystemOp
        )
        {
            Debug.Assert(2 == validLocations.Length);
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificateLocationSysErr,
                        certificateLocation,
                        certificatePath,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificateLocation,
                        certificateLocation,
                        certificatePath,
                        validLocations[0],
                        validLocations[1],
                        @"/"
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidCertificateStore(
            string certificateStore,
            string certificatePath,
            string validCertificateStore,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificateStoreSysErr,
                        certificateStore,
                        certificatePath,
                        validCertificateStore
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_InvalidCertificateStore,
                        certificateStore,
                        certificatePath,
                        validCertificateStore
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception EmptyCertificateThumbprint(
            string certificatePath,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCertificateThumbprintSysErr, certificatePath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_EmptyCertificateThumbprint, certificatePath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception CertificateNotFound(
            string thumbprint,
            string certificateLocation,
            string certificateStore,
            bool isSystemOp
        )
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_CertificateNotFoundSysErr,
                        thumbprint,
                        certificateLocation,
                        certificateStore
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(
                        Res.TCE_CertificateNotFound,
                        thumbprint,
                        certificateLocation,
                        certificateStore
                    ),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        internal static Exception InvalidAlgorithmVersionInEncryptedCEK(byte actual, byte expected)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidAlgorithmVersionInEncryptedCEK,
                    actual.ToString(@"X2"),
                    expected.ToString(@"X2")
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidCiphertextLengthInEncryptedCEK(
            int actual,
            int expected,
            string certificateName
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidCiphertextLengthInEncryptedCEK,
                    actual,
                    expected,
                    certificateName
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidCiphertextLengthInEncryptedCEKCsp(
            int actual,
            int expected,
            string masterKeyPath
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidCiphertextLengthInEncryptedCEKCsp,
                    actual,
                    expected,
                    masterKeyPath
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidCiphertextLengthInEncryptedCEKCng(
            int actual,
            int expected,
            string masterKeyPath
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidCiphertextLengthInEncryptedCEKCng,
                    actual,
                    expected,
                    masterKeyPath
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidSignatureInEncryptedCEK(
            int actual,
            int expected,
            string masterKeyPath
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidSignatureInEncryptedCEK,
                    actual,
                    expected,
                    masterKeyPath
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidSignatureInEncryptedCEKCsp(
            int actual,
            int expected,
            string masterKeyPath
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidSignatureInEncryptedCEKCsp,
                    actual,
                    expected,
                    masterKeyPath
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidSignatureInEncryptedCEKCng(
            int actual,
            int expected,
            string masterKeyPath
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidSignatureInEncryptedCEKCng,
                    actual,
                    expected,
                    masterKeyPath
                ),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidCertificateSignature(string certificatePath)
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_InvalidCertificateSignature, certificatePath),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception InvalidSignature(string masterKeyPath)
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_InvalidSignature, masterKeyPath),
                TdsEnums.TCE_PARAM_ENCRYPTED_CEK
            );
        }

        internal static Exception CertificateWithNoPrivateKey(string keyPath, bool isSystemOp)
        {
            if (isSystemOp)
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_CertificateWithNoPrivateKeySysErr, keyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
            else
            {
                return ADP.Argument(
                    Res.GetString(Res.TCE_CertificateWithNoPrivateKey, keyPath),
                    TdsEnums.TCE_PARAM_MASTERKEY_PATH
                );
            }
        }

        //
        // TCE - Cryptographic Algorithms Error messages
        //
        static internal Exception NullColumnEncryptionKeySysErr()
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_ENCRYPTIONKEY,
                Res.GetString(Res.TCE_NullColumnEncryptionKeySysErr)
            );
        }

        internal static Exception InvalidKeySize(
            string algorithmName,
            int actualKeylength,
            int expectedLength
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidKeySize,
                    algorithmName,
                    actualKeylength,
                    expectedLength
                ),
                TdsEnums.TCE_PARAM_ENCRYPTIONKEY
            );
        }

        internal static Exception InvalidEncryptionType(
            string algorithmName,
            SqlClientEncryptionType encryptionType,
            params SqlClientEncryptionType[] validEncryptionTypes
        )
        {
            const string valueSeparator = @", ";
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidEncryptionType,
                    algorithmName,
                    encryptionType.ToString(),
                    string.Join(
                        valueSeparator,
                        validEncryptionTypes.Select(
                            (validEncryptionType => @"'" + validEncryptionType + @"'")
                        )
                    )
                ),
                TdsEnums.TCE_PARAM_ENCRYPTIONTYPE
            );
        }

        internal static Exception NullPlainText()
        {
            return ADP.ArgumentNull(Res.GetString(Res.TCE_NullPlainText));
        }

        internal static Exception VeryLargeCiphertext(
            long cipherTextLength,
            long maxCipherTextSize,
            long plainTextLength
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_VeryLargeCiphertext,
                    cipherTextLength,
                    maxCipherTextSize,
                    plainTextLength
                )
            );
        }

        internal static Exception NullCipherText()
        {
            return ADP.ArgumentNull(Res.GetString(Res.TCE_NullCipherText));
        }

        internal static Exception InvalidCipherTextSize(int actualSize, int minimumSize)
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_InvalidCipherTextSize, actualSize, minimumSize),
                TdsEnums.TCE_PARAM_CIPHERTEXT
            );
        }

        internal static Exception InvalidAlgorithmVersion(byte actual, byte expected)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_InvalidAlgorithmVersion,
                    actual.ToString(@"X2"),
                    expected.ToString(@"X2")
                ),
                TdsEnums.TCE_PARAM_CIPHERTEXT
            );
        }

        internal static Exception InvalidAuthenticationTag()
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_InvalidAuthenticationTag),
                TdsEnums.TCE_PARAM_CIPHERTEXT
            );
        }

        internal static Exception NullColumnEncryptionAlgorithm(string supportedAlgorithms)
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_ENCRYPTION_ALGORITHM,
                Res.GetString(Res.TCE_NullColumnEncryptionAlgorithm, supportedAlgorithms)
            );
        }

        //
        // TCE - Errors from sp_describe_parameter_encryption
        //
        static internal Exception UnexpectedDescribeParamFormat()
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_UnexpectedDescribeParamFormat,
                    "sp_describe_parameter_encryption"
                )
            );
        }

        internal static Exception InvalidEncryptionKeyOrdinal(int ordinal, int maxOrdinal)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.TCE_InvalidEncryptionKeyOrdinal,
                    "sp_describe_parameter_encryption",
                    ordinal,
                    maxOrdinal
                )
            );
        }

        internal static Exception ParamEncryptionMetadataMissing(
            string paramName,
            string procedureName
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_ParamEncryptionMetaDataMissing,
                    "sp_describe_parameter_encryption",
                    paramName,
                    procedureName
                )
            );
        }

        internal static Exception ParamInvalidForceColumnEncryptionSetting(
            string paramName,
            string procedureName
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.TCE_ParamInvalidForceColumnEncryptionSetting,
                    TdsEnums.TCE_PARAM_FORCE_COLUMN_ENCRYPTION,
                    paramName,
                    procedureName,
                    "SqlParameter"
                )
            );
        }

        internal static Exception ParamUnExpectedEncryptionMetadata(
            string paramName,
            string procedureName
        )
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.TCE_ParamUnExpectedEncryptionMetadata,
                    paramName,
                    procedureName,
                    TdsEnums.TCE_PARAM_FORCE_COLUMN_ENCRYPTION,
                    "SqlParameter"
                )
            );
        }

        internal static Exception ProcEncryptionMetadataMissing(string procedureName)
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_ProcEncryptionMetaDataMissing,
                    "sp_describe_parameter_encryption",
                    procedureName
                )
            );
        }

        //
        // TCE- Generic toplevel failures.
        //
        static internal Exception GetExceptionArray(
            string serverName,
            string errorMessage,
            Exception e
        )
        {
            // Create and throw an exception array
            SqlErrorCollection sqlErs = new SqlErrorCollection();
            Exception exceptionToInclude = (null != e.InnerException) ? e.InnerException : e;
            sqlErs.Add(
                new SqlError(
                    infoNumber: 0,
                    errorState: (byte)0x00,
                    errorClass: (byte)TdsEnums.MIN_ERROR_CLASS,
                    server: serverName,
                    errorMessage: errorMessage,
                    procedure: null,
                    lineNumber: 0
                )
            );

            if (e is SqlException)
            {
                SqlException exThrown = (SqlException)e;
                SqlErrorCollection errorList = exThrown.Errors;
                for (int i = 0; i < exThrown.Errors.Count; i++)
                {
                    sqlErs.Add(errorList[i]);
                }
            }
            else
            {
                sqlErs.Add(
                    new SqlError(
                        infoNumber: 0,
                        errorState: (byte)0x00,
                        errorClass: (byte)TdsEnums.MIN_ERROR_CLASS,
                        server: serverName,
                        errorMessage: e.Message,
                        procedure: null,
                        lineNumber: 0
                    )
                );
            }

            return SqlException.CreateException(sqlErs, "", null, exceptionToInclude);
        }

        internal static Exception ParamEncryptionFailed(
            string paramName,
            string serverName,
            Exception e
        )
        {
            return GetExceptionArray(
                serverName,
                Res.GetString(Res.TCE_ParamEncryptionFailed, paramName),
                e
            );
        }

        internal static Exception ParamDecryptionFailed(
            string paramName,
            string serverName,
            Exception e
        )
        {
            return GetExceptionArray(
                serverName,
                Res.GetString(Res.TCE_ParamDecryptionFailed, paramName),
                e
            );
        }

        internal static Exception ColumnDecryptionFailed(
            string columnName,
            string serverName,
            Exception e
        )
        {
            return GetExceptionArray(
                serverName,
                Res.GetString(Res.TCE_ColumnDecryptionFailed, columnName),
                e
            );
        }

        //
        // TCE- Client side query processing errors.
        //
        static internal Exception UnknownColumnEncryptionAlgorithm(
            string algorithmName,
            string supportedAlgorithms
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_UnknownColumnEncryptionAlgorithm,
                    algorithmName,
                    supportedAlgorithms
                )
            );
        }

        internal static Exception UnknownColumnEncryptionAlgorithmId(
            int algoId,
            string supportAlgorithmIds
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_UnknownColumnEncryptionAlgorithmId,
                    algoId,
                    supportAlgorithmIds
                ),
                TdsEnums.TCE_PARAM_CIPHER_ALGORITHM_ID
            );
        }

        internal static Exception UnsupportedNormalizationVersion(byte version)
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_UnsupportedNormalizationVersion, version, "'1'", "SQL Server")
            );
        }

        internal static Exception UnrecognizedKeyStoreProviderName(
            string providerName,
            List<string> systemProviders,
            List<string> customProviders
        )
        {
            const string valueSeparator = @", ";
            string systemProviderStr = string.Join(
                valueSeparator,
                systemProviders.Select(provider => @"'" + provider + @"'")
            );
            string customProviderStr = string.Join(
                valueSeparator,
                customProviders.Select(provider => @"'" + provider + @"'")
            );
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_UnrecognizedKeyStoreProviderName,
                    providerName,
                    systemProviderStr,
                    customProviderStr
                )
            );
        }

        internal static Exception InvalidDataTypeForEncryptedParameter(
            string parameterName,
            int actualDataType,
            int expectedDataType
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.TCE_NullProviderValue,
                    parameterName,
                    actualDataType,
                    expectedDataType
                )
            );
        }

#if !MONO
        static internal Exception KeyDecryptionFailed(
            string providerName,
            string keyHex,
            Exception e
        )
        {
            if (providerName.Equals(SqlColumnEncryptionCertificateStoreProvider.ProviderName))
            {
                return GetExceptionArray(
                    null,
                    Res.GetString(Res.TCE_KeyDecryptionFailedCertStore, providerName, keyHex),
                    e
                );
            }
            else
            {
                return GetExceptionArray(
                    null,
                    Res.GetString(Res.TCE_KeyDecryptionFailed, providerName, keyHex),
                    e
                );
            }
        }
#endif

        static internal Exception UntrustedKeyPath(string keyPath, string serverName)
        {
            return ADP.Argument(Res.GetString(Res.TCE_UntrustedKeyPath, keyPath, serverName));
        }

        internal static Exception UnsupportedDatatypeEncryption(string dataType)
        {
            return ADP.Argument(Res.GetString(Res.TCE_UnsupportedDatatype, dataType));
        }

        internal static Exception ThrowDecryptionFailed(string keyStr, string valStr, Exception e)
        {
            return GetExceptionArray(
                null,
                Res.GetString(Res.TCE_DecryptionFailed, keyStr, valStr),
                e
            );
        }

        //
        // TCE- SQL connection related error messages
        //
        static internal Exception TceNotSupported()
        {
            return ADP.InvalidOperation(Res.GetString(Res.TCE_NotSupportedByServer, "SQL Server"));
        }

        //
        // TCE- Extensibility related error messages
        //
        static internal Exception CanOnlyCallOnce()
        {
            return ADP.InvalidOperation(Res.GetString(Res.TCE_CanOnlyCallOnce));
        }

        internal static Exception NullCustomKeyStoreProviderDictionary()
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_CLIENT_KEYSTORE_PROVIDERS,
                Res.GetString(Res.TCE_NullCustomKeyStoreProviderDictionary)
            );
        }

        internal static Exception InvalidCustomKeyStoreProviderName(
            string providerName,
            string prefix
        )
        {
            return ADP.Argument(
                Res.GetString(Res.TCE_InvalidCustomKeyStoreProviderName, providerName, prefix),
                TdsEnums.TCE_PARAM_CLIENT_KEYSTORE_PROVIDERS
            );
        }

        internal static Exception NullProviderValue(string providerName)
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_CLIENT_KEYSTORE_PROVIDERS,
                Res.GetString(Res.TCE_NullProviderValue, providerName)
            );
        }

        internal static Exception EmptyProviderName()
        {
            return ADP.ArgumentNull(
                TdsEnums.TCE_PARAM_CLIENT_KEYSTORE_PROVIDERS,
                Res.GetString(Res.TCE_EmptyProviderName)
            );
        }

        //
        // transactions.
        //
        static internal Exception ConnectionDoomed()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ConnectionDoomed));
        }

        internal static Exception OpenResultCountExceeded()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_OpenResultCountExceeded));
        }

        //
        // Global Transactions.
        //
        static internal Exception GlobalTransactionsNotEnabled()
        {
            return ADP.InvalidOperation(Res.GetString(Res.GT_Disabled));
        }

        internal static Exception UnsupportedSysTxForGlobalTransactions()
        {
            return ADP.InvalidOperation(Res.GetString(Res.GT_UnsupportedSysTxVersion));
        }

        internal static readonly byte[] AttentionHeader = new byte[]
        {
            TdsEnums.MT_ATTN, // Message Type
            TdsEnums.ST_EOM, // Status
            TdsEnums.HEADER_LEN >> 8, // length - upper byte
            TdsEnums.HEADER_LEN & 0xff, // length - lower byte
            0, // spid
            0, // spid
            0, // packet (out of band)
            0, // window
        };

        //
        // MultiSubnetFailover
        //

        /// <summary>
        /// used to block two scenarios if MultiSubnetFailover is true:
        /// * server-provided failover partner - raising SqlException in this case
        /// * connection string with failover partner and MultiSubnetFailover=true - rasing argument one in this case with the same message
        /// </summary>
        static internal Exception MultiSubnetFailoverWithFailoverPartner(
            bool serverProvidedFailoverPartner,
            SqlInternalConnectionTds internalConnection
        )
        {
            string msg = Res.GetString(Res.SQLMSF_FailoverPartnerNotSupported);
            if (serverProvidedFailoverPartner)
            {
                // VSTFDEVDIV\DevDiv2\179041 - replacing InvalidOperation with SQL exception
                SqlErrorCollection errors = new SqlErrorCollection();
                errors.Add(
                    new SqlError(0, (byte)0x00, TdsEnums.FATAL_ERROR_CLASS, null, msg, "", 0)
                );
                SqlException exc = SqlException.CreateException(errors, null, internalConnection);
                exc._doNotReconnect = true; // disable open retry logic on this error
                return exc;
            }
            else
            {
                return ADP.Argument(msg);
            }
        }

#if !MONO
        static internal Exception MultiSubnetFailoverWithMoreThan64IPs()
        {
            string msg = GetSNIErrorMessage(
                (int)SNINativeMethodWrapper.SniSpecialErrors.MultiSubnetFailoverWithMoreThan64IPs
            );
            return ADP.InvalidOperation(msg);
        }

        internal static Exception MultiSubnetFailoverWithInstanceSpecified()
        {
            string msg = GetSNIErrorMessage(
                (int)
                    SNINativeMethodWrapper.SniSpecialErrors.MultiSubnetFailoverWithInstanceSpecified
            );
            return ADP.Argument(msg);
        }

        internal static Exception MultiSubnetFailoverWithNonTcpProtocol()
        {
            string msg = GetSNIErrorMessage(
                (int)SNINativeMethodWrapper.SniSpecialErrors.MultiSubnetFailoverWithNonTcpProtocol
            );
            return ADP.Argument(msg);
        }
#endif

        //
        // Read-only routing
        //

        static internal Exception ROR_FailoverNotSupportedConnString()
        {
            return ADP.Argument(Res.GetString(Res.SQLROR_FailoverNotSupported));
        }

        internal static Exception ROR_FailoverNotSupportedServer(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    (byte)0x00,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.SQLROR_FailoverNotSupported)),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, null, internalConnection);
            exc._doNotReconnect = true;
            return exc;
        }

        internal static Exception ROR_RecursiveRoutingNotSupported(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    (byte)0x00,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.SQLROR_RecursiveRoutingNotSupported)),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, null, internalConnection);
            exc._doNotReconnect = true;
            return exc;
        }

        internal static Exception ROR_UnexpectedRoutingInfo(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    (byte)0x00,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.SQLROR_UnexpectedRoutingInfo)),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, null, internalConnection);
            exc._doNotReconnect = true;
            return exc;
        }

        internal static Exception ROR_InvalidRoutingInfo(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    (byte)0x00,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.SQLROR_InvalidRoutingInfo)),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, null, internalConnection);
            exc._doNotReconnect = true;
            return exc;
        }

        internal static Exception ROR_TimeoutAfterRoutingInfo(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    (byte)0x00,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    (Res.GetString(Res.SQLROR_TimeoutAfterRoutingInfo)),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, null, internalConnection);
            exc._doNotReconnect = true;
            return exc;
        }

        //
        // Connection resiliency
        //
        static internal SqlException CR_ReconnectTimeout()
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    TdsEnums.TIMEOUT_EXPIRED,
                    (byte)0x00,
                    TdsEnums.MIN_ERROR_CLASS,
                    null,
                    SQLMessage.Timeout(),
                    "",
                    0,
                    TdsEnums.SNI_WAIT_TIMEOUT
                )
            );
            SqlException exc = SqlException.CreateException(errors, "");
            return exc;
        }

        internal static SqlException CR_ReconnectionCancelled()
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.MIN_ERROR_CLASS,
                    null,
                    SQLMessage.OperationCancelled(),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "");
            return exc;
        }

        internal static Exception CR_NextAttemptWillExceedQueryTimeout(
            SqlException innerException,
            Guid connectionId
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.MIN_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_NextAttemptWillExceedQueryTimeout),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(
                errors,
                "",
                connectionId,
                innerException
            );
            return exc;
        }

        internal static Exception CR_EncryptionChanged(SqlInternalConnectionTds internalConnection)
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_EncryptionChanged),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "", internalConnection);
            return exc;
        }

        internal static SqlException CR_AllAttemptsFailed(
            SqlException innerException,
            Guid connectionId
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.MIN_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_AllAttemptsFailed),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(
                errors,
                "",
                connectionId,
                innerException
            );
            return exc;
        }

        internal static SqlException CR_NoCRAckAtReconnection(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_NoCRAckAtReconnection),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "", internalConnection);
            return exc;
        }

        internal static SqlException CR_TDSVersionNotPreserved(
            SqlInternalConnectionTds internalConnection
        )
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_TDSVestionNotPreserved),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "", internalConnection);
            return exc;
        }

        internal static SqlException CR_UnrecoverableServer(Guid connectionId)
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_UnrecoverableServer),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "", connectionId);
            return exc;
        }

        internal static SqlException CR_UnrecoverableClient(Guid connectionId)
        {
            SqlErrorCollection errors = new SqlErrorCollection();
            errors.Add(
                new SqlError(
                    0,
                    0,
                    TdsEnums.FATAL_ERROR_CLASS,
                    null,
                    Res.GetString(Res.SQLCR_UnrecoverableClient),
                    "",
                    0
                )
            );
            SqlException exc = SqlException.CreateException(errors, "", connectionId);
            return exc;
        }

        //
        // Merged Provider
        //
        static internal Exception BatchedUpdatesNotAvailableOnContextConnection()
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_BatchedUpdatesNotAvailableOnContextConnection)
            );
        }

        internal static Exception ContextAllowsLimitedKeywords()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ContextAllowsLimitedKeywords));
        }

        internal static Exception ContextAllowsOnlyTypeSystem2005()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ContextAllowsOnlyTypeSystem2005));
        }

        internal static Exception ContextConnectionIsInUse()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ContextConnectionIsInUse));
        }

        internal static Exception ContextUnavailableOutOfProc()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ContextUnavailableOutOfProc));
        }

        internal static Exception ContextUnavailableWhileInProc()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_ContextUnavailableWhileInProc));
        }

        internal static Exception NestedTransactionScopesNotSupported()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_NestedTransactionScopesNotSupported));
        }

        internal static Exception NotAvailableOnContextConnection()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_NotAvailableOnContextConnection));
        }

        internal static Exception NotificationsNotAvailableOnContextConnection()
        {
            return ADP.InvalidOperation(
                Res.GetString(Res.SQL_NotificationsNotAvailableOnContextConnection)
            );
        }

#if !MONO
        static internal Exception UnexpectedSmiEvent(
            Microsoft.SqlServer.Server.SmiEventSink_Default.UnexpectedEventType eventType
        )
        {
            Debug.Assert(false, "UnexpectedSmiEvent: " + eventType.ToString()); // Assert here, because these exceptions will most likely be eaten by the server.
            return ADP.InvalidOperation(Res.GetString(Res.SQL_UnexpectedSmiEvent, (int)eventType));
        }
#endif

        static internal Exception UserInstanceNotAvailableInProc()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_UserInstanceNotAvailableInProc));
        }

        internal static Exception ArgumentLengthMismatch(string arg1, string arg2)
        {
            return ADP.Argument(Res.GetString(Res.SQL_ArgumentLengthMismatch, arg1, arg2));
        }

        internal static Exception InvalidSqlDbTypeOneAllowedType(
            SqlDbType invalidType,
            string method,
            SqlDbType allowedType
        )
        {
            return ADP.Argument(
                Res.GetString(
                    Res.SQL_InvalidSqlDbTypeWithOneAllowedType,
                    invalidType,
                    method,
                    allowedType
                )
            );
        }

        internal static Exception SqlPipeErrorRequiresSendEnd()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SQL_PipeErrorRequiresSendEnd));
        }

        internal static Exception TooManyValues(string arg)
        {
            return ADP.Argument(Res.GetString(Res.SQL_TooManyValues), arg);
        }

        internal static Exception StreamWriteNotSupported()
        {
            return ADP.NotSupported(Res.GetString(Res.SQL_StreamWriteNotSupported));
        }

        internal static Exception StreamReadNotSupported()
        {
            return ADP.NotSupported(Res.GetString(Res.SQL_StreamReadNotSupported));
        }

        internal static Exception StreamSeekNotSupported()
        {
            return ADP.NotSupported(Res.GetString(Res.SQL_StreamSeekNotSupported));
        }

        internal static System.Data.SqlTypes.SqlNullValueException SqlNullValue()
        {
            System.Data.SqlTypes.SqlNullValueException e =
                new System.Data.SqlTypes.SqlNullValueException();
            ADP.TraceExceptionAsReturnValue(e);
            return e;
        }

        // SQLBU 402363: Exception to prevent Parameter.Size data corruption case from working.
        //  This should be temporary until changing to correct behavior can be safely implemented.
        static internal Exception ParameterSizeRestrictionFailure(int index)
        {
            return ADP.InvalidOperation(
                Res.GetString(
                    Res.OleDb_CommandParameterError,
                    index.ToString(CultureInfo.InvariantCulture),
                    "SqlParameter.Size"
                )
            );
        }

        internal static Exception SubclassMustOverride()
        {
            return ADP.InvalidOperation(Res.GetString(Res.SqlMisc_SubclassMustOverride));
        }

#if !MONO
        /// <summary>
        /// gets a message for SNI error (sniError must be valid, non-zero error code)
        /// </summary>
        static internal string GetSNIErrorMessage(int sniError)
        {
            Debug.Assert(
                sniError > 0
                    && sniError <= (int)SNINativeMethodWrapper.SniSpecialErrors.MaxErrorValue,
                "SNI error is out of range"
            );

            string errorMessageId = String.Format((IFormatProvider)null, "SNI_ERROR_{0}", sniError);
            return Res.GetString(errorMessageId);
        }
#endif

        // BulkLoad
        internal const string WriteToServer = "WriteToServer";

        // Default values for SqlDependency and SqlNotificationRequest
        internal const int SqlDependencyTimeoutDefault = 0;
        internal const int SqlDependencyServerTimeout = 5 * 24 * 3600; // 5 days - used to compute default TTL of the dependency
        internal const string SqlNotificationServiceDefault = "SqlQueryNotificationService";
        internal const string SqlNotificationStoredProcedureDefault =
            "SqlQueryNotificationStoredProcedure";

        // constant strings
        internal const string Transaction = "Transaction";
        internal const string Connection = "Connection";
    }

    internal sealed class SQLMessage
    {
        //

        private SQLMessage()
        { /* prevent utility class from being insantiated*/
        }

        // The class SQLMessage defines the error messages that are specific to the SqlDataAdapter
        // that are caused by a netlib error.  The functions will be called and then return the
        // appropriate error message from the resource Framework.txt.  The SqlDataAdapter will then
        // take the error message and then create a SqlError for the message and then place
        // that into a SqlException that is either thrown to the user or cached for throwing at
        // a later time.  This class is used so that there will be compile time checking of error
        // messages.  The resource Framework.txt will ensure proper string text based on the appropriate
        // locale.

        static internal string CultureIdError()
        {
            return Res.GetString(Res.SQL_CultureIdError);
        }

        internal static string EncryptionNotSupportedByClient()
        {
            return Res.GetString(Res.SQL_EncryptionNotSupportedByClient);
        }

        internal static string EncryptionNotSupportedByServer()
        {
            return Res.GetString(Res.SQL_EncryptionNotSupportedByServer);
        }

        internal static string OperationCancelled()
        {
            return Res.GetString(Res.SQL_OperationCancelled);
        }

        internal static string SevereError()
        {
            return Res.GetString(Res.SQL_SevereError);
        }

        internal static string SSPIInitializeError()
        {
            return Res.GetString(Res.SQL_SSPIInitializeError);
        }

        internal static string SSPIGenerateError()
        {
            return Res.GetString(Res.SQL_SSPIGenerateError);
        }

        internal static string Timeout()
        {
            return Res.GetString(Res.SQL_Timeout_Execution);
        }

        internal static string Timeout_PreLogin_Begin()
        {
            return Res.GetString(Res.SQL_Timeout_PreLogin_Begin);
        }

        internal static string Timeout_PreLogin_InitializeConnection()
        {
            return Res.GetString(Res.SQL_Timeout_PreLogin_InitializeConnection);
        }

        internal static string Timeout_PreLogin_SendHandshake()
        {
            return Res.GetString(Res.SQL_Timeout_PreLogin_SendHandshake);
        }

        internal static string Timeout_PreLogin_ConsumeHandshake()
        {
            return Res.GetString(Res.SQL_Timeout_PreLogin_ConsumeHandshake);
        }

        internal static string Timeout_Login_Begin()
        {
            return Res.GetString(Res.SQL_Timeout_Login_Begin);
        }

        internal static string Timeout_Login_ProcessConnectionAuth()
        {
            return Res.GetString(Res.SQL_Timeout_Login_ProcessConnectionAuth);
        }

        internal static string Timeout_PostLogin()
        {
            return Res.GetString(Res.SQL_Timeout_PostLogin);
        }

        internal static string Timeout_FailoverInfo()
        {
            return Res.GetString(Res.SQL_Timeout_FailoverInfo);
        }

        internal static string Timeout_RoutingDestination()
        {
            return Res.GetString(Res.SQL_Timeout_RoutingDestinationInfo);
        }

        internal static string Duration_PreLogin_Begin(long PreLoginBeginDuration)
        {
            return Res.GetString(Res.SQL_Duration_PreLogin_Begin, PreLoginBeginDuration);
        }

        internal static string Duration_PreLoginHandshake(
            long PreLoginBeginDuration,
            long PreLoginHandshakeDuration
        )
        {
            return Res.GetString(
                Res.SQL_Duration_PreLoginHandshake,
                PreLoginBeginDuration,
                PreLoginHandshakeDuration
            );
        }

        internal static string Duration_Login_Begin(
            long PreLoginBeginDuration,
            long PreLoginHandshakeDuration,
            long LoginBeginDuration
        )
        {
            return Res.GetString(
                Res.SQL_Duration_Login_Begin,
                PreLoginBeginDuration,
                PreLoginHandshakeDuration,
                LoginBeginDuration
            );
        }

        internal static string Duration_Login_ProcessConnectionAuth(
            long PreLoginBeginDuration,
            long PreLoginHandshakeDuration,
            long LoginBeginDuration,
            long LoginAuthDuration
        )
        {
            return Res.GetString(
                Res.SQL_Duration_Login_ProcessConnectionAuth,
                PreLoginBeginDuration,
                PreLoginHandshakeDuration,
                LoginBeginDuration,
                LoginAuthDuration
            );
        }

        internal static string Duration_PostLogin(
            long PreLoginBeginDuration,
            long PreLoginHandshakeDuration,
            long LoginBeginDuration,
            long LoginAuthDuration,
            long PostLoginDuration
        )
        {
            return Res.GetString(
                Res.SQL_Duration_PostLogin,
                PreLoginBeginDuration,
                PreLoginHandshakeDuration,
                LoginBeginDuration,
                LoginAuthDuration,
                PostLoginDuration
            );
        }

        internal static string UserInstanceFailure()
        {
            return Res.GetString(Res.SQL_UserInstanceFailure);
        }

        internal static string PreloginError()
        {
            return Res.GetString(Res.Snix_PreLogin);
        }

        internal static string ExClientConnectionId()
        {
            return Res.GetString(Res.SQL_ExClientConnectionId);
        }

        internal static string ExErrorNumberStateClass()
        {
            return Res.GetString(Res.SQL_ExErrorNumberStateClass);
        }

        internal static string ExOriginalClientConnectionId()
        {
            return Res.GetString(Res.SQL_ExOriginalClientConnectionId);
        }

        internal static string ExRoutingDestination()
        {
            return Res.GetString(Res.SQL_ExRoutingDestination);
        }
    }

    /// <summary>
    /// This class holds helper methods to escape Microsoft SQL Server identifiers, such as table, schema, database or other names
    /// </summary>
    static internal class SqlServerEscapeHelper
    {
        /// <summary>
        /// Escapes the identifier with square brackets. The input has to be in unescaped form, like the parts received from MultipartIdentifier.ParseMultipartIdentifier.
        /// </summary>
        /// <param name="name">name of the identifier, in unescaped form</param>
        /// <returns>escapes the name with [], also escapes the last close bracket with double-bracket</returns>
        static internal string EscapeIdentifier(string name)
        {
            Debug.Assert(!ADP.IsEmpty(name), "null or empty identifiers are not allowed");
            return "[" + name.Replace("]", "]]") + "]";
        }

        /// <summary>
        /// Same as above EscapeIdentifier, except that output is written into StringBuilder
        /// </summary>
        static internal void EscapeIdentifier(StringBuilder builder, string name)
        {
            Debug.Assert(builder != null, "builder cannot be null");
            Debug.Assert(!ADP.IsEmpty(name), "null or empty identifiers are not allowed");

            builder.Append("[");
            builder.Append(name.Replace("]", "]]"));
            builder.Append("]");
        }

        /// <summary>
        ///  Escape a string to be used inside TSQL literal, such as N'somename' or 'somename'
        /// </summary>
        static internal string EscapeStringAsLiteral(string input)
        {
            Debug.Assert(input != null, "input string cannot be null");
            return input.Replace("'", "''");
        }

        /// <summary>
        /// Escape a string as a TSQL literal, wrapping it around with single quotes.
        /// Use this method to escape input strings to prevent SQL injection
        /// and to get correct behavior for embedded quotes.
        /// </summary>
        /// <param name="input">unescaped string</param>
        /// <returns>escaped and quoted literal string</returns>
        static internal string MakeStringLiteral(string input)
        {
            if (ADP.IsEmpty(input))
            {
                return "''";
            }
            else
            {
                return "'" + EscapeStringAsLiteral(input) + "'";
            }
        }
    }

    /// <summary>
    /// This class holds methods invoked on System.Transactions through reflection for Global Transactions
    /// </summary>
    static internal class SysTxForGlobalTransactions
    {
        private static readonly Lazy<MethodInfo> _enlistPromotableSinglePhase =
            new Lazy<MethodInfo>(
                () =>
                    typeof(SysTx.Transaction).GetMethod(
                        "EnlistPromotableSinglePhase",
                        new Type[]
                        {
                            typeof(SysTx.IPromotableSinglePhaseNotification),
                            typeof(Guid),
                        }
                    )
            );

        private static readonly Lazy<MethodInfo> _setDistributedTransactionIdentifier =
            new Lazy<MethodInfo>(
                () =>
                    typeof(SysTx.Transaction).GetMethod(
                        "SetDistributedTransactionIdentifier",
                        new Type[]
                        {
                            typeof(SysTx.IPromotableSinglePhaseNotification),
                            typeof(Guid),
                        }
                    )
            );

        private static readonly Lazy<MethodInfo> _getPromotedToken = new Lazy<MethodInfo>(
            () => typeof(SysTx.Transaction).GetMethod("GetPromotedToken")
        );

        /// <summary>
        /// Enlists the given IPromotableSinglePhaseNotification and Non-MSDTC Promoter type into a transaction
        /// </summary>
        /// <returns>The MethodInfo instance to be invoked. Null if the method doesn't exist</returns>
        public static MethodInfo EnlistPromotableSinglePhase
        {
            get { return _enlistPromotableSinglePhase.Value; }
        }

        /// <summary>
        /// Sets the given DistributedTransactionIdentifier for a Transaction instance.
        /// Needs to be invoked when using a Non-MSDTC Promoter type
        /// </summary>
        /// <returns>The MethodInfo instance to be invoked. Null if the method doesn't exist</returns>
        public static MethodInfo SetDistributedTransactionIdentifier
        {
            get { return _setDistributedTransactionIdentifier.Value; }
        }

        /// <summary>
        /// Gets the Promoted Token for a Transaction
        /// </summary>
        /// <returns>The MethodInfo instance to be invoked. Null if the method doesn't exist</returns>
        public static MethodInfo GetPromotedToken
        {
            get { return _getPromotedToken.Value; }
        }
    }
} //namespace
