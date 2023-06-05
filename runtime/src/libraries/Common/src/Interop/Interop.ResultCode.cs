partial
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class Interop
{
    partial internal static class Globalization
    {
        // needs to be kept in sync with ResultCode in System.Globalization.Native
        internal enum ResultCode
        {
            Success = 0,
            UnknownError = 1,
            InsufficientBuffer = 2,
            OutOfMemory = 3
        }
    }
}
