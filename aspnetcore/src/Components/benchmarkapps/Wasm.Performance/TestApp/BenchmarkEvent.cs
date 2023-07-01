using Microsoft.JSInterop;

namespace Wasm.Performance.TestApp;

public static class BenchmarkEvent
{
    public static void Send(IJSRuntime jsRuntime, string name)
    {
        // jsRuntime will be null if we're in an environment without any
        // JS runtime, e.g., the console runner
        ((IJSInProcessRuntime)jsRuntime)?.Invoke<object>("receiveBenchmarkEvent", name);
    }
}
