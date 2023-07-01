using System.Runtime.InteropServices;
using Microsoft.JSInterop;

namespace WebAssembly.JSInterop;

[StructLayout(LayoutKind.Explicit, Pack = 4)]
internal struct JSCallInfo
{
    [FieldOffset(0)]
    public string FunctionIdentifier;

    [FieldOffset(4)]
    public JSCallResultType ResultType;

    [FieldOffset(8)]
    public string MarshalledCallArgsJson;

    [FieldOffset(12)]
    public long MarshalledCallAsyncHandle;

    [FieldOffset(20)]
    public long TargetInstanceId;
}
