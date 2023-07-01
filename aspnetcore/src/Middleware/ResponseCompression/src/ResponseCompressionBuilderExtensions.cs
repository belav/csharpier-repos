using Microsoft.AspNetCore.ResponseCompression;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for the ResponseCompression middleware.
/// </summary>
public static class ResponseCompressionBuilderExtensions
{
    /// <summary>
    /// Adds middleware for dynamically compressing HTTP Responses.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
    public static IApplicationBuilder UseResponseCompression(this IApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.UseMiddleware<ResponseCompressionMiddleware>();
    }
}
