using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Components.WebView;

/// <summary>
/// Used to look up MIME types given a file path
/// </summary>
internal interface IContentTypeProvider
{
    /// <summary>
    /// Given a file path, determine the MIME type
    /// </summary>
    /// <param name="subpath">A file path</param>
    /// <param name="contentType">The resulting MIME type</param>
    /// <returns>True if MIME type could be determined</returns>
    bool TryGetContentType(string subpath, [MaybeNullWhen(false)] out string contentType);
}
