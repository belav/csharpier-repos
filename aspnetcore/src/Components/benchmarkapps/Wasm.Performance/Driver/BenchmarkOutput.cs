// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Wasm.Performance.Driver;

internal class BenchmarkOutput
{
    public List<BenchmarkMetadata> Metadata { get; } = new List<BenchmarkMetadata>();

    public List<BenchmarkMeasurement> Measurements { get; } = new List<BenchmarkMeasurement>();
}
