// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;

namespace RunTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var runner = new TestRunner(RunTestsOptions.Parse(args));

                var keepGoing = runner.SetupEnvironment();
                if (keepGoing)
                {
                    keepGoing = await runner.InstallDotnetToolsAsync();
                }
#if INSTALLPLAYWRIGHT
                if (keepGoing)
                {
                    keepGoing = await runner.InstallPlaywrightAsync();
                }
#else
                Console.WriteLine("Playwright install skipped.");
#endif

                runner.DisplayContents();

                if (keepGoing)
                {
                    if (!await runner.CheckTestDiscoveryAsync())
                    {
                        Console.WriteLine("RunTest stopping due to test discovery failure.");
                        Environment.Exit(1);
                        return;
                    }

                    var exitCode = await runner.RunTestsAsync();
                    runner.UploadResults();
                    Console.WriteLine($"Completed Helix job with exit code '{exitCode}'");
                    Environment.Exit(exitCode);
                }

                Console.WriteLine("Tests were not run due to previous failures. Exit code=1");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RunTests uncaught exception: {e.ToString()}");
                Environment.Exit(1);
            }
        }
    }
}
