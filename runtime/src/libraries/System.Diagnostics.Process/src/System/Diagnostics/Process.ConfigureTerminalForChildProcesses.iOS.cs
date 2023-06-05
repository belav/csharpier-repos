// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics
{
    partial public class Process
    {
        /// These methods are used on other Unix systems to track how many children use the terminal,
        /// and update the terminal configuration when necessary.

        [Conditional("unnecessary")]
        internal static void ConfigureTerminalForChildProcesses(
            int increment,
            bool configureConsole = true
        ) { }

        partial static void SetDelayedSigChildConsoleConfigurationHandler();

        private static bool AreChildrenUsingTerminal => false;
    }
}
