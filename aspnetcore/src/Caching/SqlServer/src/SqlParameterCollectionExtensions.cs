// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Microsoft.Extensions.Caching.SqlServer;

internal static class SqlParameterCollectionExtensions
{
    // For all values where the length is less than the below value, try setting the size of the
    // parameter for better performance.
    public const int DefaultValueColumnWidth = 8000;

    // Maximum size of a primary key column is 900 bytes (898 bytes from the key + 2 additional bytes required by
    // the Sql Server).
    public const int CacheItemIdColumnWidth = 449;

    public static SqlParameterCollection AddCacheItemId(
        this SqlParameterCollection parameters,
        string value
    )
    {
        return parameters.AddWithValue(
            Columns.Names.CacheItemId,
            SqlDbType.NVarChar,
            CacheItemIdColumnWidth,
            value
        );
    }

    public static SqlParameterCollection AddCacheItemValue(
        this SqlParameterCollection parameters,
        byte[]? value
    )
    {
        if (value != null && value.Length < DefaultValueColumnWidth)
        {
            return parameters.AddWithValue(
                Columns.Names.CacheItemValue,
                SqlDbType.VarBinary,
                DefaultValueColumnWidth,
                value
            );
        }
        else
        {
            // do not mention the size
            return parameters.AddWithValue(
                Columns.Names.CacheItemValue,
                SqlDbType.VarBinary,
                value
            );
        }
    }

    public static SqlParameterCollection AddSlidingExpirationInSeconds(
        this SqlParameterCollection parameters,
        TimeSpan? value
    )
    {
        if (value.HasValue)
        {
            return parameters.AddWithValue(
                Columns.Names.SlidingExpirationInSeconds,
                SqlDbType.BigInt,
                value.Value.TotalSeconds
            );
        }
        else
        {
            return parameters.AddWithValue(
                Columns.Names.SlidingExpirationInSeconds,
                SqlDbType.BigInt,
                DBNull.Value
            );
        }
    }

    public static SqlParameterCollection AddAbsoluteExpiration(
        this SqlParameterCollection parameters,
        DateTimeOffset? utcTime
    )
    {
        if (utcTime.HasValue)
        {
            return parameters.AddWithValue(
                Columns.Names.AbsoluteExpiration,
                SqlDbType.DateTimeOffset,
                utcTime.Value
            );
        }
        else
        {
            return parameters.AddWithValue(
                Columns.Names.AbsoluteExpiration,
                SqlDbType.DateTimeOffset,
                DBNull.Value
            );
        }
    }

    public static SqlParameterCollection AddWithValue(
        this SqlParameterCollection parameters,
        string parameterName,
        SqlDbType dbType,
        object? value
    )
    {
        var parameter = new SqlParameter(parameterName, dbType);
        parameter.Value = value;
        parameters.Add(parameter);
        parameter.ResetSqlDbType();
        return parameters;
    }

    public static SqlParameterCollection AddWithValue(
        this SqlParameterCollection parameters,
        string parameterName,
        SqlDbType dbType,
        int size,
        object? value
    )
    {
        var parameter = new SqlParameter(parameterName, dbType, size);
        parameter.Value = value;
        parameters.Add(parameter);
        parameter.ResetSqlDbType();
        return parameters;
    }
}
