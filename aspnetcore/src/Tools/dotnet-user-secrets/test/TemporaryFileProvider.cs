using System;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.SecretManager.Tools.Tests;

internal class TemporaryFileProvider : IDisposable
{
    public TemporaryFileProvider()
    {
        Root = Directory
            .CreateDirectory(
                Path.Combine(Path.GetTempPath(), "tmpfiles", Guid.NewGuid().ToString())
            )
            .FullName;
    }

    public string Root { get; }

    public void Add(string filename, string contents)
    {
        File.WriteAllText(Path.Combine(Root, filename), contents, Encoding.UTF8);
    }

    public void Dispose()
    {
        Directory.Delete(Root, recursive: true);
    }
}
