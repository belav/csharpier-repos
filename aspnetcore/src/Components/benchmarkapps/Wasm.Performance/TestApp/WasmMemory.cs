using Microsoft.JSInterop;

namespace Wasm.Performance.TestApp;

public static class WasmMemory
{
    [JSInvokable]
    public static long GetTotalMemory() => GC.GetTotalMemory(forceFullCollection: true);
}
