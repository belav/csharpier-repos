using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Identity.DefaultUI.WebSite;

/// <summary>
/// Provides version hash for a specified file.
/// </summary>
internal class FileVersionProvider : IFileVersionProvider
{
    public string AddFileVersionToPath(PathString requestPathBase, string path) => path;
}
