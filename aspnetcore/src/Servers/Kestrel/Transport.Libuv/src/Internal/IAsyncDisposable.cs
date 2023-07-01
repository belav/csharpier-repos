using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal
{
    interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
