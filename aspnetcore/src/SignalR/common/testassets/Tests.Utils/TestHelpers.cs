using System.Net.WebSockets;

namespace Microsoft.AspNetCore.SignalR.Tests;

public static class TestHelpers
{
    public static bool IsWebSocketsSupported()
    {
        try
        {
            new ClientWebSocket().Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
