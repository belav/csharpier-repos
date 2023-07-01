using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;

namespace Microsoft.AspNetCore.SignalR.Crankier
{
    public interface IWorker
    {
        Task PingAsync(int value);
        Task ConnectAsync(
            string targetAddress,
            HttpTransportType transportType,
            int numberOfConnections
        );
        Task StartTestAsync(TimeSpan sendInterval, int sendBytes);
        Task StopAsync();
    }
}
