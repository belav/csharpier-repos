// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Net.Test.Common
{
    partial public static class Configuration
    {
        partial public static class Sockets
        {
            public static Uri SocketServer =>
                GetUriValue(
                    "DOTNET_TEST_NET_SOCKETS_SERVERURI",
                    new Uri("http://" + DefaultAzureServer)
                );

            public static string InvalidHost =>
                GetValue(
                    "DOTNET_TEST_NET_SOCKETS_INVALIDSERVER",
                    "notahostname.invalid.corp.microsoft.com"
                );
        }
    }
}
