using System.IO.Compression;

namespace Microsoft.AspNetCore.RequestDecompression;

/// <summary>
/// Brotli decompression provider.
/// </summary>
internal sealed class BrotliDecompressionProvider : IDecompressionProvider
{
    /// <inheritdoc />
    public Stream GetDecompressionStream(Stream stream)
    {
        return new BrotliStream(stream, CompressionMode.Decompress, leaveOpen: true);
    }
}
