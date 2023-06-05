using System;

namespace MonoTests.SystemWeb.Framework
{
    partial public class WebTest
    {
        partial static void CopyResourcesLocal()
        {
            Type myself = typeof(WebTest);
#if !DOTNET
            CopyResource(myself, "Web.mono.config", "Web.config");
#endif
#if NET_4_6
            CopyResource(myself, "profile.config.4.x", "profile.config");
#else
#error "Unknown profile"
#endif
        }
    }
}
