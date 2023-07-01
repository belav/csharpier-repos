using System.Diagnostics;

namespace Microsoft.AspNetCore.Razor.Language;

// Internal for testing
[DebuggerDisplay("{Path}")]
internal class FileNode
{
    public FileNode(string path, RazorProjectItem projectItem)
    {
        Path = path;
        ProjectItem = projectItem;
    }

    public DirectoryNode Directory { get; set; }

    public string Path { get; }

    public RazorProjectItem ProjectItem { get; set; }
}
