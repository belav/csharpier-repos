using System.Buffers;

namespace Microsoft.AspNetCore.Server.IIS.Core.IO;

internal interface IAsyncIOEngine
{
    ValueTask<int> ReadAsync(Memory<byte> memory);
    ValueTask<int> WriteAsync(ReadOnlySequence<byte> data);
    ValueTask FlushAsync(bool moreData);
    void NotifyCompletion(int hr, int bytes);
    void Complete();
}
