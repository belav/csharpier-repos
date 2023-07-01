using Microsoft.AspNetCore.RequestDecompression;

namespace RequestDecompressionSample;

public class CustomDecompressionProvider : IDecompressionProvider
{
    public Stream GetDecompressionStream(Stream stream)
    {
        // Create a custom decompression stream wrapper here.
        return stream;
    }
}
