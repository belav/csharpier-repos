// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sample
{
    partial
    // http://localhost:8000/?task=AppStart
    public class AppStartTask : BenchTask
    {
        public override string Name => "AppStart";
        public override bool BrowserOnly => true;

        public AppStartTask()
        {
            measurements = new Measurement[] { new PageShow(), new ReachManaged(), };
        }

        Measurement[] measurements;
        public override Measurement[] Measurements => measurements;

        class PageShow : BenchTask.Measurement
        {
            public override string Name => "Page show";

            public override int InitialSamples => 3;

            public override bool HasRunStepAsync => true;

            public override async Task RunStepAsync()
            {
                await MainApp.PageShow();
            }
        }

        class ReachManaged : BenchTask.Measurement
        {
            public override string Name => "Reach managed";
            public override int InitialSamples => 3;
            public override bool HasRunStepAsync => true;

            public override async Task RunStepAsync()
            {
                await MainApp.FrameReachedManaged();
            }
        }

        partial public class MainApp
        {
            [JSImport("globalThis.mainApp.PageShow")]
            partial public static Task PageShow();

            [JSImport("globalThis.mainApp.FrameReachedManaged")]
            partial public static Task FrameReachedManaged();
        }

        partial public class FrameApp
        {
            [JSImport("globalThis.frameApp.ReachedCallback")]
            partial public static Task ReachedCallback();

            [JSExport]
            public static void ReachedManaged()
            {
                ReachedCallback();
            }
        }
    }
}
