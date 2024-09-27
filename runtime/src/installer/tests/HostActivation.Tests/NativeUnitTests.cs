// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using FluentAssertions;
using Microsoft.DotNet.Cli.Build.Framework;
using Microsoft.DotNet.CoreSetup.Test;
using Xunit;

namespace Microsoft.DotNet.CoreSetup.Test.HostActivation.NativeUnitTests
{
    public class NativeUnitTests
    {
        [Fact]
        public void Native_Test_Fx_Ver()
        {
            RepoDirectoriesProvider repoDirectoriesProvider = new RepoDirectoriesProvider();

            string testPath = Path.Combine(
                repoDirectoriesProvider.HostTestArtifacts,
                Binaries.GetExeFileNameForCurrentPlatform("test_fx_ver")
            );

            Command testCommand = Command.Create(testPath);
            testCommand.Execute().Should().Pass();
        }
    }
}
