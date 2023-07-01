using System.Threading.Tasks;

namespace Microsoft.AspNetCore.SignalR.Crankier
{
    public interface IAgent
    {
        Task PongAsync(int id, int value);
        Task LogAsync(int id, string text);
        Task StatusAsync(int id, StatusInformation statusInformation);
    }
}
