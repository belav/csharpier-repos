// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Tools.Internal;

namespace Microsoft.Extensions.ApiDescription.Tool.Commands;

internal abstract class ProjectCommandBase : HelpCommandBase
{
    public ProjectCommandBase(IConsole console)
        : base(console) { }

    public CommandOption AssemblyPath { get; private set; }

    public CommandOption ProjectName { get; private set; }

    public CommandOption ToolsDirectory { get; private set; }

    public override void Configure(CommandLineApplication command)
    {
        base.Configure(command);

        AssemblyPath = command.Option("--assembly <PATH>", Resources.AssemblyDescription);
        ProjectName = command.Option("--project <Name>", Resources.ProjectDescription);
        ToolsDirectory = command.Option(
            "--tools-directory <PATH>",
            Resources.ToolsDirectoryDescription
        );
    }

    protected override void Validate()
    {
        base.Validate();

        if (!AssemblyPath.HasValue())
        {
            throw new CommandException(Resources.FormatMissingOption(AssemblyPath.LongName));
        }

        if (!ProjectName.HasValue())
        {
            throw new CommandException(Resources.FormatMissingOption(ProjectName.LongName));
        }

        if (!ToolsDirectory.HasValue())
        {
            throw new CommandException(Resources.FormatMissingOption(ToolsDirectory.LongName));
        }
    }
}
