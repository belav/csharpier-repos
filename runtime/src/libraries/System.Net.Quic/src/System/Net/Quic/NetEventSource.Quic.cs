// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Tracing;
using System.Net.Quic;

namespace System.Net
{
    [EventSource(Name = "Private.InternalDiagnostics.System.Net.Quic")]
    partial internal sealed class NetEventSource
    {
        partial static void AdditionalCustomizedToString(object value, ref string? result)
        {
            if (value is MsQuicSafeHandle safeHandle)
            {
                result = safeHandle.ToString();
            }
        }
    }
}
