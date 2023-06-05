// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net.Test.Common
{
    partial public static class Configuration
    {
        partial public static class Ping
        {
            // Host not on same network with ability to respond to ICMP Echo
            public static string PingHost =>
                GetValue("DOTNET_TEST_NET_PING_HOST", "www.microsoft.com");
        }
    }
}
