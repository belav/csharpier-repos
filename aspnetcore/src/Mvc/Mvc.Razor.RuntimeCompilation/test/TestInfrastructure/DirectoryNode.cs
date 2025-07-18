﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace Microsoft.AspNetCore.Razor.Language;

// Internal for testing
[DebuggerDisplay("{Path}")]
internal class DirectoryNode
{
    public DirectoryNode(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public List<DirectoryNode> Directories { get; } = new List<DirectoryNode>();

    public List<FileNode> Files { get; } = new List<FileNode>();

    public void AddFile(FileNode fileNode)
    {
        var filePath = fileNode.Path;
        if (!filePath.StartsWith(Path, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"File {fileNode.Path} does not belong to {Path}.");
        }

        // Look for the first / that appears in the path after the current directory path.
        var directoryPath = GetDirectoryPath(filePath);
        var directory = GetOrAddDirectory(this, directoryPath, createIfNotExists: true);
        Debug.Assert(directory != null);
        directory.Files.Add(fileNode);
        fileNode.Directory = directory;
    }

    public DirectoryNode GetDirectory(string path)
    {
        if (!path.StartsWith(Path, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"File {path} does not belong to {Path}.");
        }

        return GetOrAddDirectory(this, path);
    }

    public IEnumerable<RazorProjectItem> EnumerateItems()
    {
        foreach (var file in Files)
        {
            yield return file.ProjectItem;
        }

        foreach (var directory in Directories)
        {
            foreach (var file in directory.EnumerateItems())
            {
                yield return file;
            }
        }
    }

    public RazorProjectItem GetItem(string path)
    {
        if (!path.StartsWith(Path, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"File {path} does not belong to {Path}.");
        }

        var directoryPath = GetDirectoryPath(path);
        var directory = GetOrAddDirectory(this, directoryPath);
        if (directory == null)
        {
            return new NotFoundProjectItem("/", path);
        }

        foreach (var file in directory.Files)
        {
            var filePath = file.Path;
            var directoryLength = directory.Path.Length;

            // path, filePath -> /Views/Home/Index.cshtml
            // directory.Path -> /Views/Home/
            // We only need to match the file name portion since we've already matched the directory segment.
            if (
                string.Compare(
                    path,
                    directoryLength,
                    filePath,
                    directoryLength,
                    path.Length - directoryLength,
                    StringComparison.OrdinalIgnoreCase
                ) == 0
            )
            {
                return file.ProjectItem;
            }
        }

        return new NotFoundProjectItem("/", path);
    }

    private static string GetDirectoryPath(string path)
    {
        // /dir1/dir2/file.cshtml -> /dir1/dir2/
        var fileNameIndex = path.LastIndexOf('/');
        if (fileNameIndex == -1)
        {
            return path;
        }

        return path.Substring(0, fileNameIndex + 1);
    }

    private static DirectoryNode GetOrAddDirectory(
        DirectoryNode directory,
        string path,
        bool createIfNotExists = false
    )
    {
        Debug.Assert(!string.IsNullOrEmpty(path));
        if (path[path.Length - 1] != '/')
        {
            path += '/';
        }

        int index;
        while ((index = path.IndexOf('/', directory.Path.Length)) != -1 && index != path.Length)
        {
            var subDirectory = FindSubDirectory(directory, path);

            if (subDirectory == null)
            {
                if (createIfNotExists)
                {
                    var directoryPath = path.Substring(0, index + 1); // + 1 to include trailing slash
                    subDirectory = new DirectoryNode(directoryPath);
                    directory.Directories.Add(subDirectory);
                }
                else
                {
                    return null;
                }
            }

            directory = subDirectory;
        }

        return directory;
    }

    private static DirectoryNode FindSubDirectory(DirectoryNode parentDirectory, string path)
    {
        for (var i = 0; i < parentDirectory.Directories.Count; i++)
        {
            // ParentDirectory.Path -> /Views/Home/
            // CurrentDirectory.Path -> /Views/Home/SubDir/
            // Path -> /Views/Home/SubDir/MorePath/File.cshtml
            // Each invocation of FindSubDirectory returns the immediate subdirectory along the path to the file.

            var currentDirectory = parentDirectory.Directories[i];
            var directoryPath = currentDirectory.Path;
            var startIndex = parentDirectory.Path.Length;

            if (
                string.Compare(
                    path,
                    startIndex,
                    directoryPath,
                    startIndex,
                    directoryPath.Length - startIndex,
                    StringComparison.OrdinalIgnoreCase
                ) == 0
            )
            {
                return currentDirectory;
            }
        }

        return null;
    }
}
