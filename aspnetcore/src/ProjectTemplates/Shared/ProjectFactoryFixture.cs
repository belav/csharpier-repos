// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Templates.Test.Helpers
{
    public class ProjectFactoryFixture : IDisposable
    {
        private readonly ConcurrentDictionary<string, Project> _projects = new ConcurrentDictionary<string, Project>();

        public IMessageSink DiagnosticsMessageSink { get; }

        public ProjectFactoryFixture(IMessageSink diagnosticsMessageSink)
        {
            DiagnosticsMessageSink = diagnosticsMessageSink;
        }

        public async Task<Project> GetOrCreateProject(string projectKey, ITestOutputHelper output)
        {
            await TemplatePackageInstaller.EnsureTemplatingEngineInitializedAsync(output);
            // Different tests may have different output helpers, so need to fix up the output to write to the correct log
            if (_projects.TryGetValue(projectKey, out var project))
            {
                project.Output = output;
                return project;
            }
            return _projects.GetOrAdd(
                projectKey,
                (key, outputHelper) =>
                {
                    var project = new Project
                    {
                        Output = outputHelper,
                        DiagnosticsMessageSink = DiagnosticsMessageSink,
                        ProjectGuid = Path.GetRandomFileName().Replace(".", string.Empty)
                    };
                    project.ProjectName = $"AspNet.{project.ProjectGuid}";

                    var assemblyPath = GetType().Assembly;
                    var basePath = GetTemplateFolderBasePath(assemblyPath);
                    project.TemplateOutputDir = Path.Combine(basePath, project.ProjectName);
                    return project;
                },
                output);
        }

        private static string GetTemplateFolderBasePath(Assembly assembly) =>
            (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HELIX_DIR")))
            ? assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(a => a.Key == "TestTemplateCreationFolder")
                .Value
            : Path.Combine(Environment.GetEnvironmentVariable("HELIX_DIR"), "Templates", "BaseFolder");

        public void Dispose()
        {
            var list = new List<Exception>();
            foreach (var project in _projects)
            {
                try
                {
                    project.Value.Dispose();
                }
                catch (Exception e)
                {
                    list.Add(e);
                }
            }

            if (list.Count > 0)
            {
                throw new AggregateException(list);
            }
        }
    }
}
