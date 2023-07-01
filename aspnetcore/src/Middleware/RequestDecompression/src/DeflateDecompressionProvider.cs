using System.IO.Compression;

namespace Microsoft.AspNetCore.RequestDecompression;

/// <summary>
/// DEFLATE decompression provider.
/// </summary>
internal sealed class DeflateDecompressionProvider : IDecompressionProvider
{
    /// <inheritdoc />
    public Stream GetDecompressionStream(Stream stream)
    {
        return new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
    }
}
