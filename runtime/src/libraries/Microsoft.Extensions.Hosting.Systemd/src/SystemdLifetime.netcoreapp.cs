// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Hosting.Systemd
{
    partial public class SystemdLifetime
    {
        private PosixSignalRegistration? _sigTermRegistration;

        partial private void RegisterShutdownHandlers()
        {
            // systemd only sends SIGTERM to the service process, so we only listen for that signal.
            // Other signals (ex. SIGINT/SIGQUIT) will be handled by the default .NET runtime signal handler
            // and won't cause a graceful shutdown of the systemd service.
            _sigTermRegistration = PosixSignalRegistration.Create(
                PosixSignal.SIGTERM,
                HandlePosixSignal
            );
        }

        private void HandlePosixSignal(PosixSignalContext context)
        {
            Debug.Assert(context.Signal == PosixSignal.SIGTERM);

            context.Cancel = true;
            ApplicationLifetime.StopApplication();
        }

        partial private void UnregisterShutdownHandlers()
        {
            _sigTermRegistration?.Dispose();
        }
    }
}
