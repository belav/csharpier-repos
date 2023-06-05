// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.Tools.Commands;

partial
// ReSharper disable once ArrangeTypeModifiers
internal class DatabaseUpdateCommand
{
    protected override int Execute(string[] args)
    {
        using var executor = CreateExecutor(args);

        executor.UpdateDatabase(_migration!.Value, _connection!.Value(), Context!.Value());

        return base.Execute(args);
    }
}
