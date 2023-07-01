using System;

namespace Microsoft.Extensions.FileProviders.Embedded.Manifest.Task;

public class EmbeddedItem : IEquatable<EmbeddedItem>
{
    public string ManifestFilePath { get; set; }

    public string AssemblyResourceName { get; set; }

    public bool Equals(EmbeddedItem other) =>
        string.Equals(ManifestFilePath, other?.ManifestFilePath, StringComparison.Ordinal)
        && string.Equals(
            AssemblyResourceName,
            other?.AssemblyResourceName,
            StringComparison.Ordinal
        );

    public override bool Equals(object obj) => Equals(obj as EmbeddedItem);

    public override int GetHashCode() =>
        ManifestFilePath.GetHashCode() ^ AssemblyResourceName.GetHashCode();
}
