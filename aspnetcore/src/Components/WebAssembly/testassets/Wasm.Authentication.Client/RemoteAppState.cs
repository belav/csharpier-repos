using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Wasm.Authentication.Client;

public class RemoteAppState : RemoteAuthenticationState
{
    public string State { get; set; }
}
