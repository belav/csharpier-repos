﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Microsoft.AspNetCore.SpaProxy;

internal class SpaDevelopmentServerOptions
{
    public string ServerUrl { get; set; } = "";

    public string LaunchCommand { get; set; } = "";

    public int MaxTimeoutInSeconds { get; set; }

    public TimeSpan MaxTimeout => TimeSpan.FromSeconds(MaxTimeoutInSeconds);

    public string WorkingDirectory { get; set; } = "";
}
