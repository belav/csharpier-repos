// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.ServiceProcess
{
    partial public class ServiceBase : System.ComponentModel.Component
    {
        public void RequestAdditionalTime(System.TimeSpan time) { }
    }

    partial public class ServiceController : System.ComponentModel.Component
    {
        public void Stop(bool stopDependentServices) { }
    }

    partial public readonly struct SessionChangeDescription
        : System.IEquatable<System.ServiceProcess.SessionChangeDescription> { }
}
