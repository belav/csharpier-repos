using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR.Crankier
{
    public interface IRunner
    {
        Task PongWorkerAsync(int workerId, int value);

        Task LogAgentAsync(string format, params object[] arguments);

        Task LogWorkerAsync(int workerId, string format, params object[] arguments);
    }
}
