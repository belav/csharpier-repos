using System;

namespace MonoTests.SystemWeb.Framework
{
    partial public class WebTest
    {
        partial static void CopyResourcesLocal()
        {
            Type myself = typeof(WebTest);
            CopyPrefixedResources(myself, "App_GlobalResources/", "App_GlobalResources");
            CopyPrefixedResources(myself, "App_Code/", "App_Code");
#if DOTNET
            CopyResource(myself, "Web.config", "Web.config");
#else
            CopyResource(myself, "Web.mono.config.4.0", "Web.config");
#endif
        }
    }
}
