// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.TestUtilities;

public static class DatabaseFacadeExtensions
{
    public static bool EnsureCreatedResiliently(this DatabaseFacade fa�ade)
        => fa�ade.CreateExecutionStrategy().Execute(fa�ade, f => f.EnsureCreated());

    public static Task<bool> EnsureCreatedResilientlyAsync(this DatabaseFacade fa�ade, CancellationToken cancellationToken = default)
        => fa�ade.CreateExecutionStrategy().ExecuteAsync(fa�ade, (f, ct) => f.EnsureCreatedAsync(ct), cancellationToken);
}
