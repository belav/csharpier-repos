//------------------------------------------------------------------------------
// <copyright file="OdbcEnvironment.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace System.Data.Odbc
{
    internal sealed class OdbcEnvironment
    {
        private static object _globalEnvironmentHandle;
        private static object _globalEnvironmentHandleLock = new object();

        private OdbcEnvironment() { } // default const.

        internal static OdbcEnvironmentHandle GetGlobalEnvironmentHandle()
        {
            OdbcEnvironmentHandle globalEnvironmentHandle =
                _globalEnvironmentHandle as OdbcEnvironmentHandle;
            if (null == globalEnvironmentHandle)
            {
                ADP.CheckVersionMDAC(true);

                lock (_globalEnvironmentHandleLock)
                {
                    globalEnvironmentHandle = _globalEnvironmentHandle as OdbcEnvironmentHandle;
                    if (null == globalEnvironmentHandle)
                    {
                        globalEnvironmentHandle = new OdbcEnvironmentHandle();
                        _globalEnvironmentHandle = globalEnvironmentHandle;
                    }
                }
            }
            return globalEnvironmentHandle;
        }

        internal static void ReleaseObjectPool()
        {
            object globalEnvironmentHandle = Interlocked.Exchange(
                ref _globalEnvironmentHandle,
                null
            );
            if (null != globalEnvironmentHandle)
            {
                (globalEnvironmentHandle as OdbcEnvironmentHandle).Dispose(); // internally refcounted so will happen correctly
            }
        }
    }
}
