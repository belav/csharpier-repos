using System.Buffers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Microsoft.AspNetCore.SignalR.Tests;

public class WriteThenCloseConnectionHandler : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var result = await connection.Transport.Input.ReadAsync();
        var buffer = result.Buffer;

        if (!buffer.IsEmpty)
        {
            await connection.Transport.Output.WriteAsync(buffer.ToArray());
        }

        connection.Transport.Input.AdvanceTo(buffer.End);
    }
}
