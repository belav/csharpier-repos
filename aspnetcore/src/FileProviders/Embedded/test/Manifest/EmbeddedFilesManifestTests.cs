using Xunit;

namespace Microsoft.Extensions.FileProviders.Embedded.Manifest;

public class EmbeddedFilesManifestTests
{
    [Theory]
    [InlineData("/wwwroot//jquery.validate.js")]
    [InlineData("//wwwroot/jquery.validate.js")]
    public void ResolveEntry_IgnoresInvalidPaths(string path)
    {
        // Arrange
        var manifest = new EmbeddedFilesManifest(
            ManifestDirectory.CreateRootDirectory(
                new[]
                {
                    ManifestDirectory.CreateDirectory(
                        "wwwroot",
                        new[]
                        {
                            new ManifestFile("jquery.validate.js", "wwwroot.jquery.validate.js")
                        }
                    )
                }
            )
        );
        // Act
        var entry = manifest.ResolveEntry(path);

        // Assert
        Assert.Null(entry);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("./")]
    [InlineData("/wwwroot/jquery.validate.js")]
    [InlineData("/wwwroot/")]
    public void ResolveEntry_AllowsSingleDirectorySeparator(string path)
    {
        // Arrange
        var manifest = new EmbeddedFilesManifest(
            ManifestDirectory.CreateRootDirectory(
                new[]
                {
                    ManifestDirectory.CreateDirectory(
                        "wwwroot",
                        new[]
                        {
                            new ManifestFile("jquery.validate.js", "wwwroot.jquery.validate.js")
                        }
                    )
                }
            )
        );
        // Act
        var entry = manifest.ResolveEntry(path);

        // Assert
        Assert.NotNull(entry);
    }
}
