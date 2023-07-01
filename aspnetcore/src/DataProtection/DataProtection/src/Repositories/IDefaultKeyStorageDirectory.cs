using System.IO;

namespace Microsoft.AspNetCore.DataProtection.Repositories;

/// <summary>
/// This interface enables overridding the default storage location of keys on disk
/// </summary>
internal interface IDefaultKeyStorageDirectories
{
    DirectoryInfo? GetKeyStorageDirectory();

    DirectoryInfo? GetKeyStorageDirectoryForAzureWebSites();
}
