using System;
using System.Net.Quic;

namespace Microsoft.AspNetCore.Testing;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class MsQuicSupportedAttribute : Attribute, ITestCondition
{
#pragma warning disable CA2252 // This API requires opting into preview features
    public bool IsMet => QuicListener.IsSupported;
#pragma warning restore CA2252 // This API requires opting into preview features

    public string SkipReason => "QUIC is not supported on the current test machine";
}
